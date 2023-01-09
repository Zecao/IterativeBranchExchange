function t_pf(quiet)
%T_PF  Tests for power flow solvers.

%   MATPOWER
%   Copyright (c) 2004-2019, Power Systems Engineering Research Center (PSERC)
%   by Ray Zimmerman, PSERC Cornell
%
%   This file is part of MATPOWER.
%   Covered by the 3-clause BSD License (see LICENSE file for details).
%   See https://matpower.org for more info.

if nargin < 1
    quiet = 0;
end

AC_alg = {'NR', 'NR-SC', 'NR-IP', 'NR-IC', 'FDXB', 'FDBX', 'GS'};
AC_name = {
    'Newton (default, power-polar)',
    'Newton (power-cartesian)',
    'Newton (current-polar)',
    'Newton (current-cartesian)',
    'Fast Decoupled (XB)',
    'Fast Decoupled (BX)',
    'Gauss-Seidel'
};

t_begin(length(AC_alg)*44 + 14, quiet);

casefile = 't_case9_pf';
if quiet
    verbose = 0;
else
    verbose = 1;
end
if have_fcn('octave')
    if have_fcn('octave', 'vnum') >= 4
        file_in_path_warn_id = 'Octave:data-file-in-path';
    else
        file_in_path_warn_id = 'Octave:load-file-in-path';
    end
    s1 = warning('query', file_in_path_warn_id);
    warning('off', file_in_path_warn_id);
end
mpopt0 = mpoption('out.all', 0, 'pf.tol', 1e-9, 'verbose', 0);
mpopt0 = mpoption(mpopt0, 'verbose', verbose);

%% define named indices into bus, gen, branch matrices
[PQ, PV, REF, NONE, BUS_I, BUS_TYPE, PD, QD, GS, BS, BUS_AREA, VM, ...
    VA, BASE_KV, ZONE, VMAX, VMIN, LAM_P, LAM_Q, MU_VMAX, MU_VMIN] = idx_bus;
[F_BUS, T_BUS, BR_R, BR_X, BR_B, RATE_A, RATE_B, RATE_C, ...
    TAP, SHIFT, BR_STATUS, PF, QF, PT, QT, MU_SF, MU_ST, ...
    ANGMIN, ANGMAX, MU_ANGMIN, MU_ANGMAX] = idx_brch;
[GEN_BUS, PG, QG, QMAX, QMIN, VG, MBASE, GEN_STATUS, PMAX, PMIN, ...
    MU_PMAX, MU_PMIN, MU_QMAX, MU_QMIN, PC1, PC2, QC1MIN, QC1MAX, ...
    QC2MIN, QC2MAX, RAMP_AGC, RAMP_10, RAMP_30, RAMP_Q, APF] = idx_gen;

%% network with islands
mpc0 = loadcase(casefile);
mpc0.gen(1, PG) = 60;
mpc0.gen(1, [PMIN PMAX QMIN QMAX PG QG]) = mpc0.gen(1, [PMIN PMAX QMIN QMAX PG QG]) / 2;
mpc0.gen = [mpc0.gen(1, :); mpc0.gen];
mpc1 = mpc0;
mpc  = mpc0;
nb = size(mpc.bus, 1);
mpc1.bus(:, BUS_I)      = mpc1.bus(:, BUS_I) + nb;
mpc1.branch(:, F_BUS)   = mpc1.branch(:, F_BUS) + nb;
mpc1.branch(:, T_BUS)   = mpc1.branch(:, T_BUS) + nb;
mpc1.gen(:, GEN_BUS)    = mpc1.gen(:, GEN_BUS) + nb;
mpc.bus         = [mpc.bus; mpc1.bus];
mpc.branch      = [mpc.branch; mpc1.branch];
mpc.gen         = [mpc.gen; mpc1.gen];
mpc1 = mpc;

%%-----  AC power flow  -----
%% get solved AC power flow case from MAT-file
load soln9_pf;      %% defines bus_soln, gen_soln, branch_soln

%% run AC PF
for k = 1:length(AC_alg)
    t = sprintf('AC PF - %s : ', AC_name{k});
    mpopt = mpoption(mpopt0, 'pf.alg', AC_alg{k});
    [baseMVA, bus, gen, branch, success, et] = runpf(casefile, mpopt);
    t_ok(success, [t 'success']);
    t_is(bus, bus_soln, 6, [t 'bus']);
    t_is(gen, gen_soln, 6, [t 'gen']);
    t_is(branch, branch_soln, 6, [t 'branch']);

    r = runpf(casefile, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.bus, bus_soln, 6, [t 'bus']);
    t_is(r.gen, gen_soln, 6, [t 'gen']);
    t_is(r.branch, branch_soln, 6, [t 'branch']);

    %% check Qg distribution, when Qmin = Qmax
    t = sprintf('%s - check Qg : ', AC_alg{k});
    mpopt = mpoption(mpopt, 'pf.alg', AC_alg{k}, 'verbose', 0);
    mpc = loadcase(casefile);
    mpc.gen(1, [QMIN QMAX]) = [20 20];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1, QG), 24.07, 2, [t 'single gen, Qmin = Qmax']);

    mpc.gen = [mpc.gen(1, :); mpc.gen];
    mpc.gen(1, [QMIN QMAX]) = [10 10];
    mpc.gen(2, [QMIN QMAX]) = [0 50];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1:2, QG), [10; 14.07], 2, [t '2 gens, Qmin = Qmax for one']);

    mpc.gen(1, [QMIN QMAX]) = [10 10];
    mpc.gen(2, [QMIN QMAX]) = [-50 -50];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1:2, QG), [42.03; -17.97], 2, [t '2 gens, Qmin = Qmax for both']);

    mpc.gen(1, [QMIN QMAX]) = [0 50];
    mpc.gen(2, [QMIN QMAX]) = [0 100];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1:2, QG), [8.02; 16.05], 2, [t '2 gens, proportional']);

    mpc.gen(1, [QMIN QMAX]) = [-50 0];
    mpc.gen(2, [QMIN QMAX]) = [50 150];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1:2, QG), [-50+8.02; 50+16.05], 2, [t '2 gens, proportional']);

    mpc.gen(1, [QMIN QMAX]) = [-50 Inf];
    mpc.gen(2, [QMIN QMAX]) = [50 150];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1:2, QG), [-31.61; 55.68], 2, [t '2 gens, one infinite range']);

    mpc.gen(1, [QMIN QMAX]) = [-50 Inf];
    mpc.gen(2, [QMIN QMAX]) = [50 Inf];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1:2, QG), [-33.12; 57.18], 2, [t '2 gens, both infinite range']);

    mpc.gen(1, [QMIN QMAX]) = [-50 Inf];
    mpc.gen(2, [QMIN QMAX]) = [-Inf 150];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1:2, QG), [76.07; -52], 2, [t '2 gens, both infinite range']);

    mpc.gen(1, [QMIN QMAX]) = [-Inf Inf];
    mpc.gen(2, [QMIN QMAX]) = [-Inf Inf];
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(1:2, QG), [12.03; 12.03], 2, [t '2 gens, both infinite range']);

    t = sprintf('%s - reactive generation allocation : ', AC_alg{k});
    mpc = loadcase(casefile);
    %% generator data
    %	bus	Pg	Qg	Qmax	Qmin	Vg	mBase	status	Pmax	Pmin	Pc1	Pc2	Qc1min	Qc1max	Qc2min	Qc2max	ramp_agc	ramp_10	ramp_30	ramp_q	apf
    mpc.gen = [
		1	0	0	300	-300	1	100	1	250	10	0	0	0	0	0	0	0	0	0	0	0;
		2	54	0	0	-5	1	100	1	300	10	0	0	0	0	0	0	0	0	0	0	0;
		2	54	0	5	-5	1	100	1	300	10	0	0	0	0	0	0	0	0	0	0	0;
		2	55	0	25	 10	1	100	1	300	10	0	0	0	0	0	0	0	0	0	0	0;
		30	25	1	300	-300	1	100	1	270	10	0	0	0	0	0	0	0	0	0	0	0;
		30	30	2	300	-300	1	100	1	270	10	0	0	0	0	0	0	0	0	0	0	0;
		30	30	-3	300	-300	1	100	1	270	10	0	0	0	0	0	0	0	0	0	0	0;
    ];
    mpc.bus(3, BUS_TYPE) = PQ;
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.gen(2:4, QG), [-5; -5; 10] + [1; 2; 3]*1.989129794, 7, [t 'PV bus']);
    t_is(r.gen(5:7, QG), [1; 2; -3], 8, [t 'PQ bus']);

    %% network with islands
    t = sprintf('%s - network w/islands : AC PF : ', AC_alg{k});
    mpc = mpc1;
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.bus( 1:9,  VA), bus_soln(:, VA), 7, [t 'voltage angles 1']);
    t_is(r.bus(10:18, VA), bus_soln(:, VA), 7, [t 'voltage angles 2']);
    Pg = [gen_soln(1, PG)-30; 30; gen_soln(2:3, PG)];
    t_is(r.gen(1:4, PG), Pg, 6, [t 'active power generation 1']);
    t_is(r.gen(5:8, PG), Pg, 6, [t 'active power generation 2']);

    t = sprintf('%s - all buses isolated : ', AC_alg{k});
    mpc.bus(:, BUS_TYPE) = NONE;
    try
        r = runpf(mpc, mpopt);
        t_is(r.success, 0, 12, [t 'success = 0']);
    catch
        t_ok(0, [t 'unexpected fatal error']);
    end

    %% case 14 with Q limits
    t = sprintf('%s - pf.enforce_q_lims == 0 : ', AC_alg{k});
    mpc = loadcase('case14');
    mpc.gen(1, QMIN) = -10;
    mpc.gen(:, QMAX) = [10; 30; 29; 15; 15];
    bt0 = mpc.bus(:, BUS_TYPE);
    bt = bt0;
    mpopt = mpoption(mpopt, 'pf.enforce_q_lims', 0);
    r = runpf(mpc, mpopt);
    t_ok(r.success, [t 'success']);
    t_is(r.bus(:, BUS_TYPE), bt, 12, [t 'bus type']);
    t_is(r.gen(:, QG), [-16.549300542; 43.557100134; 25.075348495; 12.730944405; 17.623451366], 6, [t 'Qg']);

    t = sprintf('%s - pf.enforce_q_lims == 1 : ', AC_alg{k});
    mpopt = mpoption(mpopt, 'pf.enforce_q_lims', 1);
    r = runpf(mpc, mpopt);
    bt = bt0;
    bt([1 2 3 8]) = [PQ PQ REF PQ];
    t_is(r.success, 0, 12, [t 'success = 0']);
    t_is(r.bus(:, BUS_TYPE), bt, 12, [t 'bus type']);
    t_is(r.gen(:, QG), [-10; 30; 31.608422873; 16.420423190; 15], 4, [t 'Qg']);

    t = sprintf('%s - pf.enforce_q_lims == 2 : ', AC_alg{k});
    mpopt = mpoption(mpopt, 'pf.enforce_q_lims', 2);
    r = runpf(mpc, mpopt);
    bt = bt0;
    bt([1 2 3 6 8]) = [REF PQ PQ PQ PQ];
    t_ok(r.success, [t 'success']);
    t_is(r.bus(:, BUS_TYPE), bt, 12, [t 'bus type']);
    t_is(r.gen(:, QG), [-6.30936644; 30; 29; 15; 15], 6, [t 'Qg']);
end

%%-----  DC power flow  -----
mpopt = mpoption(mpopt, 'verbose', verbose);
%% get solved AC power flow case from MAT-file
load soln9_dcpf;        %% defines bus_soln, gen_soln, branch_soln

%% run DC PF
t = 'DC PF : ';
[baseMVA, bus, gen, branch, success, et] = rundcpf(casefile, mpopt);
t_ok(success, [t 'success']);
t_is(bus, bus_soln, 6, [t 'bus']);
t_is(gen, gen_soln, 6, [t 'gen']);
t_is(branch, branch_soln, 6, [t 'branch']);
r = rundcpf(casefile, mpopt);
t_ok(r.success, [t 'success']);
t_is(r.bus, bus_soln, 6, [t 'bus']);
t_is(r.gen, gen_soln, 6, [t 'gen']);
t_is(r.branch, branch_soln, 6, [t 'branch']);

%% network with islands
t = sprintf('DC PF - network w/islands : ');
mpc  = mpc1;
%mpopt = mpoption(mpopt, 'out.bus', 1, 'out.gen', 1, 'out.all', -1, 'verbose', 2);
r = rundcpf(mpc, mpopt);
t_ok(r.success, [t 'success']);
t_is(r.bus( 1:9,  VA), bus_soln(:, VA), 8, [t 'voltage angles 1']);
t_is(r.bus(10:18, VA), bus_soln(:, VA), 8, [t 'voltage angles 2']);
Pg = [gen_soln(1, PG)-30; 30; gen_soln(2:3, PG)];
t_is(r.gen(1:4, PG), Pg, 8, [t 'active power generation 1']);
t_is(r.gen(5:8, PG), Pg, 8, [t 'active power generation 1']);

%% island without slack bus (catch singluar matrix?)
t = sprintf('DC PF - network w/islands w/o slack : ');
k = find(mpc.bus(:, BUS_TYPE) == REF);
mpc.bus(k(2), BUS_TYPE) = PV;
warn_state = warning;
warning('off', 'all');  %% turn of (near-)singular matrix warnings
r = rundcpf(mpc, mpopt);
warning(warn_state);
t_is(r.success, 0, 12, [t 'success = 0']);

t_end;

if have_fcn('octave')
    warning(s1.state, file_in_path_warn_id);
end
