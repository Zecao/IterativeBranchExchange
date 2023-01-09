function V0 = getV0(bus, gen, type_initialguess, V0)
%GETV0  Get initial voltage profile for power flow calculation.
%   Note: The pv bus voltage will remain at the given value even for
%   flat start.
%   type_initialguess: 1 - initial guess from case data
%                      2 - flat start
%                      3 - from input

%   MATPOWER
%   Copyright (c) 2009-2016, Power Systems Engineering Research Center (PSERC)
%   by Rui Bo
%
%   This file is part of MATPOWER/mx-se.
%   Covered by the 3-clause BSD License (see LICENSE file for details).
%   See https://github.com/MATPOWER/mx-se/ for more info.

%% define named indices into bus, gen, branch matrices
[PQ, PV, REF, NONE, BUS_I, BUS_TYPE, PD, QD, GS, BS, BUS_AREA, VM, ...
    VA, BASE_KV, ZONE, VMAX, VMIN, LAM_P, LAM_Q, MU_VMAX, MU_VMIN] = idx_bus;
[F_BUS, T_BUS, BR_R, BR_X, BR_B, RATE_A, RATE_B, ...
    RATE_C, TAP, SHIFT, BR_STATUS, PF, QF, PT, QT, MU_SF, MU_ST] = idx_brch;
[GEN_BUS, PG, QG, QMAX, QMIN, VG, MBASE, ...
    GEN_STATUS, PMAX, PMIN, MU_PMAX, MU_PMIN, MU_QMAX, MU_QMIN] = idx_gen;

%% generator info
on = find(gen(:, GEN_STATUS) > 0);      %% which generators are on?
gbus = gen(on, GEN_BUS);                %% what buses are they at?
if type_initialguess == 1 % using previous value in case data
    % NOTE: angle is in degree in case data, but in radians in pf solver,
    % so conversion from degree to radians is needed here
    V0  = bus(:, VM) .* exp(1j * pi/180 * bus(:, VA)); 
elseif type_initialguess == 2 % using flat start
    V0 = ones(size(bus, 1), 1);
elseif type_initialguess == 3 % using given initial voltage
    V0 = V0;
else
    fprintf('Error: unknow ''type_initialguess''.\n');
    pause
end
% set the voltages of PV bus and reference bus into the initial guess
V0(gbus) = gen(on, VG) ./ abs(V0(gbus)).* V0(gbus);
