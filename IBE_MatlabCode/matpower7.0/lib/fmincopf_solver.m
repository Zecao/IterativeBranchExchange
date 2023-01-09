function [results, success, raw] = fmincopf_solver(om, mpopt)
%FMINCOPF_SOLVER  Solves AC optimal power flow using FMINCON.
%
%   [RESULTS, SUCCESS, RAW] = FMINCOPF_SOLVER(OM, MPOPT)
%
%   Inputs are an OPF model object and a MATPOWER options struct.
%
%   Outputs are a RESULTS struct, SUCCESS flag and RAW output struct.
%
%   RESULTS is a MATPOWER case struct (mpc) with the usual baseMVA, bus
%   branch, gen, gencost fields, along with the following additional
%   fields:
%       .order      see 'help ext2int' for details of this field
%       .x          final value of optimization variables (internal order)
%       .f          final objective function value
%       .mu         shadow prices on ...
%           .var
%               .l  lower bounds on variables
%               .u  upper bounds on variables
%           .nln    (deprecated) 2*nb+2*nl - Pmis, Qmis, Sf, St
%               .l  lower bounds on nonlinear constraints
%               .u  upper bounds on nonlinear constraints
%           .nle    nonlinear equality constraints
%           .nli    nonlinear inequality constraints
%           .lin
%               .l  lower bounds on linear constraints
%               .u  upper bounds on linear constraints
%
%   SUCCESS     1 if solver converged successfully, 0 otherwise
%
%   RAW         raw output in form returned by MINOS
%       .xr     final value of optimization variables
%       .pimul  constraint multipliers
%       .info   solver specific termination code
%       .output solver specific output information
%
%   See also OPF, FMINCON.

%   MATPOWER
%   Copyright (c) 2000-2017, Power Systems Engineering Research Center (PSERC)
%   by Ray Zimmerman, PSERC Cornell
%   and Carlos E. Murillo-Sanchez, PSERC Cornell & Universidad Nacional de Colombia
%
%   This file is part of MATPOWER.
%   Covered by the 3-clause BSD License (see LICENSE file for details).
%   See https://matpower.org for more info.

%%----- initialization -----
%% define named indices into data matrices
[PQ, PV, REF, NONE, BUS_I, BUS_TYPE, PD, QD, GS, BS, BUS_AREA, VM, ...
    VA, BASE_KV, ZONE, VMAX, VMIN, LAM_P, LAM_Q, MU_VMAX, MU_VMIN] = idx_bus;
[GEN_BUS, PG, QG, QMAX, QMIN, VG, MBASE, GEN_STATUS, PMAX, PMIN, ...
    MU_PMAX, MU_PMIN, MU_QMAX, MU_QMIN, PC1, PC2, QC1MIN, QC1MAX, ...
    QC2MIN, QC2MAX, RAMP_AGC, RAMP_10, RAMP_30, RAMP_Q, APF] = idx_gen;
[F_BUS, T_BUS, BR_R, BR_X, BR_B, RATE_A, RATE_B, RATE_C, ...
    TAP, SHIFT, BR_STATUS, PF, QF, PT, QT, MU_SF, MU_ST, ...
    ANGMIN, ANGMAX, MU_ANGMIN, MU_ANGMAX] = idx_brch;
[PW_LINEAR, POLYNOMIAL, MODEL, STARTUP, SHUTDOWN, NCOST, COST] = idx_cost;

%% unpack data
mpc = om.get_mpc();
[baseMVA, bus, gen, branch, gencost] = ...
    deal(mpc.baseMVA, mpc.bus, mpc.gen, mpc.branch, mpc.gencost);
[vv, ll, nne, nni] = om.get_idx();

%% problem dimensions
nb = size(bus, 1);          %% number of buses
nl = size(branch, 1);       %% number of branches
ny = om.getN('var', 'y');   %% number of piece-wise linear costs

%% bounds on optimization vars
[x0, xmin, xmax] = om.params_var();

%% linear constraints
[A, l, u] = om.params_lin_constraint();

%% split l <= A*x <= u into less than, equal to, greater than, and
%% doubly-bounded sets
ieq = find( abs(u-l) <= eps );          %% equality
igt = find( u >=  1e10 & l > -1e10 );   %% greater than, unbounded above
ilt = find( l <= -1e10 & u <  1e10 );   %% less than, unbounded below
ibx = find( (abs(u-l) > eps) & (u < 1e10) & (l > -1e10) );
Af  = [ A(ilt, :); -A(igt, :); A(ibx, :); -A(ibx, :) ];
bf  = [ u(ilt);   -l(igt);     u(ibx);    -l(ibx)];
Afeq = A(ieq, :);
bfeq = u(ieq);

%% build admittance matrices
[Ybus, Yf, Yt] = makeYbus(baseMVA, bus, branch);

%% try to select an interior initial point, unless requested not to
if mpopt.opf.start < 2
    s = 1;                      %% set init point inside bounds by s
    lb = xmin; ub = xmax;
    lb(xmin == -Inf) = -1e10;   %% replace Inf with numerical proxies
    ub(xmax ==  Inf) =  1e10;
    x0 = (lb + ub) / 2;         %% set x0 mid-way between bounds
    k = find(xmin == -Inf & xmax < Inf);    %% if only bounded above
    x0(k) = xmax(k) - s;                    %% set just below upper bound
    k = find(xmin > -Inf & xmax == Inf);    %% if only bounded below
    x0(k) = xmin(k) + s;                    %% set just above lower bound
    Varefs = bus(bus(:, BUS_TYPE) == REF, VA) * (pi/180);
    Vmax = min(bus(:, VMAX), 1.5);
    Vmin = max(bus(:, VMIN), 0.5);
    Vm = (Vmax + Vmin) / 2;
    if mpopt.opf.v_cartesian
        V = Vm * exp(1j*Varefs(1));
        x0(vv.i1.Vr:vv.iN.Vr) = real(V);
        x0(vv.i1.Vi:vv.iN.Vi) = imag(V);
    else
        x0(vv.i1.Va:vv.iN.Va) = Varefs(1);  %% angles set to first reference angle
        x0(vv.i1.Vm:vv.iN.Vm) = Vm;         %% voltage magnitudes
        if ny > 0
            ipwl = find(gencost(:, MODEL) == PW_LINEAR);
            c = gencost(sub2ind(size(gencost), ipwl, NCOST+2*gencost(ipwl, NCOST)));    %% largest y-value in CCV data
            x0(vv.i1.y:vv.iN.y) = max(c) + 0.1 * abs(max(c));
        end
    end
end

%% find branches with flow limits
il = find(branch(:, RATE_A) ~= 0 & branch(:, RATE_A) < 1e10);
nl2 = length(il);           %% number of constrained lines

%% basic optimset options needed for fmincon
fmoptions = optimset('GradObj', 'on', 'GradConstr', 'on', ...
            'TolCon', mpopt.opf.violation, 'TolX', mpopt.fmincon.tol_x, ...
            'TolFun', mpopt.fmincon.tol_f );
if mpopt.fmincon.max_it ~= 0
    fmoptions = optimset(fmoptions, 'MaxIter', mpopt.fmincon.max_it, ...
            'MaxFunEvals', 4 * mpopt.fmincon.max_it);
end

if mpopt.verbose == 0,
  fmoptions.Display = 'off';
elseif mpopt.verbose == 1
  fmoptions.Display = 'iter';
else
  fmoptions.Display = 'testing';
end

%% select algorithm
if have_fcn('fmincon_ipm')
  switch mpopt.fmincon.alg
    case 1              %% active-set (does not use sparse matrices, not suitable for large problems)
      fmoptions = optimset(fmoptions, 'Algorithm', 'active-set');
      Af = full(Af);
      Afeq = full(Afeq);
    case 2              %% interior-point, w/ default 'bfgs' Hessian approx
      fmoptions = optimset(fmoptions, 'Algorithm', 'interior-point');
    case 3              %% interior-point, w/ 'lbfgs' Hessian approx
      fmoptions = optimset(fmoptions, 'Algorithm', 'interior-point', 'Hessian','lbfgs');
    case 4              %% interior-point, w/ exact user-supplied Hessian
      fmc_hessian = @(x, lambda)opf_hessfcn(x, lambda, 1, om, Ybus, Yf(il,:), Yt(il,:), mpopt, il);
      fmoptions = optimset(fmoptions, 'Algorithm', 'interior-point', ...
          'Hessian', 'user-supplied', 'HessFcn', fmc_hessian);
    case 5              %% interior-point, w/ finite-diff Hessian
      fmoptions = optimset(fmoptions, 'Algorithm', 'interior-point', 'Hessian','fin-diff-grads', 'SubProblem', 'cg');
    case 6              %% sqp (does not use sparse matrices, not suitable for large problems)
      fmoptions = optimset(fmoptions, 'Algorithm', 'sqp');
      Af = full(Af);
      Afeq = full(Afeq);
    otherwise
      error('fmincopf_solver: unknown algorithm specified in ''fmincon.alg'' option');
  end
else
  fmoptions = optimset(fmoptions, 'LargeScale', 'off');
  Af = full(Af);
  Afeq = full(Afeq);
end
% fmoptions = optimset(fmoptions, 'DerivativeCheck', 'on', 'FinDiffType', 'central', 'FunValCheck', 'on');
% fmoptions = optimset(fmoptions, 'Diagnostics', 'on');

%%-----  run opf  -----
f_fcn = @(x)opf_costfcn(x, om);
gh_fcn = @(x)opf_consfcn(x, om, Ybus, Yf(il,:), Yt(il,:), mpopt, il);
[x, f, info, Output, Lambda] = ...
  fmincon(f_fcn, x0, Af, bf, Afeq, bfeq, xmin, xmax, gh_fcn, fmoptions);
success = (info > 0);

%% update solution data
if mpopt.opf.v_cartesian
    Vi = x(vv.i1.Vi:vv.iN.Vi);
    Vr = x(vv.i1.Vr:vv.iN.Vr);
    V = Vr + 1j*Vi;
    Va = angle(V);
    Vm = abs(V);
else
    Va = x(vv.i1.Va:vv.iN.Va);
    Vm = x(vv.i1.Vm:vv.iN.Vm);
    V = Vm .* exp(1j*Va);
end
Pg = x(vv.i1.Pg:vv.iN.Pg);
Qg = x(vv.i1.Qg:vv.iN.Qg);

%%-----  calculate return values  -----
%% update voltages & generator outputs
bus(:, VA) = Va * 180/pi;
bus(:, VM) = Vm;
gen(:, PG) = Pg * baseMVA;
gen(:, QG) = Qg * baseMVA;
gen(:, VG) = Vm(gen(:, GEN_BUS));

%% compute branch flows
Sf = V(branch(:, F_BUS)) .* conj(Yf * V);  %% cplx pwr at "from" bus, p.u.
St = V(branch(:, T_BUS)) .* conj(Yt * V);  %% cplx pwr at "to" bus, p.u.
branch(:, PF) = real(Sf) * baseMVA;
branch(:, QF) = imag(Sf) * baseMVA;
branch(:, PT) = real(St) * baseMVA;
branch(:, QT) = imag(St) * baseMVA;

%% line constraint is typically on square of limit
%% so we must fix multipliers
muSf = zeros(nl, 1);
muSt = zeros(nl, 1);
if ~isempty(il)
    if upper(mpopt.opf.flow_lim(1)) == 'P'
        muSf(il) = Lambda.ineqnonlin(nni.i1.Sf:nni.iN.Sf);
        muSt(il) = Lambda.ineqnonlin(nni.i1.St:nni.iN.St);
    else
        muSf(il) = 2 * Lambda.ineqnonlin(nni.i1.Sf:nni.iN.Sf) .* branch(il, RATE_A) / baseMVA;
        muSt(il) = 2 * Lambda.ineqnonlin(nni.i1.St:nni.iN.St) .* branch(il, RATE_A) / baseMVA;
    end
end

%% fix Lambdas
%% (shadow prices on equality variable bounds can come back on wrong limit)
kl = find(Lambda.lower < 0 & Lambda.upper == 0);
Lambda.upper(kl) = -Lambda.lower(kl);
Lambda.lower(kl) = 0;
ku = find(Lambda.upper < 0 & Lambda.lower == 0);
Lambda.lower(ku) = -Lambda.upper(ku);
Lambda.upper(ku) = 0;

%% update Lagrange multipliers
if mpopt.opf.v_cartesian
    if om.userdata.veq
        lam = Lambda.eqnonlin(nne.i1.Veq:nne.iN.Veq);
        mu_Vmax = zeros(size(lam));
        mu_Vmin = zeros(size(lam));
        mu_Vmax(lam > 0) =  lam(lam > 0);
        mu_Vmin(lam < 0) = -lam(lam < 0);
        bus(om.userdata.veq, MU_VMAX) = mu_Vmax;
        bus(om.userdata.veq, MU_VMIN) = mu_Vmin;
    end
    bus(om.userdata.viq, MU_VMAX) = Lambda.ineqnonlin(nni.i1.Vmax:nni.iN.Vmax);
    bus(om.userdata.viq, MU_VMIN) = Lambda.ineqnonlin(nni.i1.Vmin:nni.iN.Vmin);
else
    bus(:, MU_VMAX)  = Lambda.upper(vv.i1.Vm:vv.iN.Vm);
    bus(:, MU_VMIN)  = Lambda.lower(vv.i1.Vm:vv.iN.Vm);
end
gen(:, MU_PMAX)  = Lambda.upper(vv.i1.Pg:vv.iN.Pg) / baseMVA;
gen(:, MU_PMIN)  = Lambda.lower(vv.i1.Pg:vv.iN.Pg) / baseMVA;
gen(:, MU_QMAX)  = Lambda.upper(vv.i1.Qg:vv.iN.Qg) / baseMVA;
gen(:, MU_QMIN)  = Lambda.lower(vv.i1.Qg:vv.iN.Qg) / baseMVA;
if mpopt.opf.current_balance
    %% convert current balance shadow prices to equivalent lamP and lamQ
    %% P + jQ = (Vr + jVi) * (M - jN)
    %% M = (Vr P + Vi Q) / (Vr^2 + Vi^2)
    %% N = (Vi P - Vr Q) / (Vr^2 + Vi^2)
    %% lamP = df/dP = df/dM * dM/dP + df/dN + dN/dP
    %% lamQ = df/dQ = df/dM * dM/dQ + df/dN + dN/dQ
    VV = V ./ (V .* conj(V));   %% V / Vm^2
    VVr = real(VV);
    VVi = imag(VV);
    lamM = Lambda.eqnonlin(nne.i1.rImis:nne.iN.rImis);
    lamN = Lambda.eqnonlin(nne.i1.iImis:nne.iN.iImis);
    bus(:, LAM_P) = (VVr.*lamM + VVi.*lamN) / baseMVA;
    bus(:, LAM_Q) = (VVi.*lamM - VVr.*lamN) / baseMVA;
else
    bus(:, LAM_P) = Lambda.eqnonlin(nne.i1.Pmis:nne.iN.Pmis) / baseMVA;
    bus(:, LAM_Q) = Lambda.eqnonlin(nne.i1.Qmis:nne.iN.Qmis) / baseMVA;
end
branch(:, MU_SF) = muSf / baseMVA;
branch(:, MU_ST) = muSt / baseMVA;

%% package up results
nlnN = 2*nb + 2*nl;     %% because muSf and muSt are nl x 1, not nl2 x 1
nlt = length(ilt);
ngt = length(igt);
nbx = length(ibx);

%% extract multipliers for nonlinear constraints
kl = find(Lambda.eqnonlin < 0);
ku = find(Lambda.eqnonlin > 0);
nl_mu_l = zeros(nlnN, 1);
nl_mu_u = [zeros(2*nb, 1); muSf; muSt];
nl_mu_l(kl) = -Lambda.eqnonlin(kl);
nl_mu_u(ku) =  Lambda.eqnonlin(ku);

%% extract multipliers for linear constraints
kl = find(Lambda.eqlin < 0);
ku = find(Lambda.eqlin > 0);

mu_l = zeros(size(u));
mu_l(ieq(kl)) = -Lambda.eqlin(kl);
mu_l(igt) = Lambda.ineqlin(nlt+(1:ngt));
mu_l(ibx) = Lambda.ineqlin(nlt+ngt+nbx+(1:nbx));

mu_u = zeros(size(u));
mu_u(ieq(ku)) = Lambda.eqlin(ku);
mu_u(ilt) = Lambda.ineqlin(1:nlt);
mu_u(ibx) = Lambda.ineqlin(nlt+ngt+(1:nbx));

mu = struct( ...
  'var', struct('l', Lambda.lower, 'u', Lambda.upper), ...
  'nln', struct('l', nl_mu_l, 'u', nl_mu_u), ...
  'nle', Lambda.eqnonlin, ...
  'nli', Lambda.ineqnonlin, ...
  'lin', struct('l', mu_l, 'u', mu_u) );

results = mpc;
[results.bus, results.branch, results.gen, ...
    results.om, results.x, results.mu, results.f] = ...
        deal(bus, branch, gen, om, x, mu, f);

pimul = [ ...
  results.mu.nln.l - results.mu.nln.u;
  results.mu.lin.l - results.mu.lin.u;
  -ones(ny>0, 1);
  results.mu.var.l - results.mu.var.u;
];
raw = struct('xr', x, 'pimul', pimul, 'info', info, 'output', Output);
