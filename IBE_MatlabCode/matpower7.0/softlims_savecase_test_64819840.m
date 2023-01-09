function mpc = softlims_savecase_test_64819840
%SOFTLIMS_SAVECASE_TEST_64819840

%% MATPOWER Case Format : Version 2
mpc.version = '2';

%%-----  Power Flow Data  -----%%
%% system MVA base
mpc.baseMVA = 100;

%% bus data
%	bus_i	type	Pd	Qd	Gs	Bs	area	Vm	Va	baseKV	zone	Vmax	Vmin
mpc.bus = [
	10	3	0	0	0	0	1	1	0	345	1	1.1	0.9;
	20	2	0	0	0	0	1	1	0	345	1	1.1	0.9;
	30	2	0	0	0	0	1	1	0	345	1	1.1	0.9;
	40	1	0	0	0	0	1	1	0	345	1	1.1	0.9;
	50	1	90	30	0	0	1	1	0	345	1	1.1	0.9;
	60	1	0	0	0	0	1	1	0	345	1	1.1	0.9;
	70	1	100	35	0	0	1	1	0	345	1	1.1	0.9;
	80	1	0	0	0	0	1	1	0	345	1	1.1	0.9;
	90	1	125	50	0	0	1	1	0	345	1	1.1	0.9;
];

%% generator data
%	bus	Pg	Qg	Qmax	Qmin	Vg	mBase	status	Pmax	Pmin	Pc1	Pc2	Qc1min	Qc1max	Qc2min	Qc2max	ramp_agc	ramp_10	ramp_30	ramp_q	apf
mpc.gen = [
	30	85	-10.95	300	-300	1.025	100	1	270	10	0	0	0	0	0	0	0	0	0	0	0;
	20	163	6.54	300	-300	1.025	100	0	300	10	0	0	0	0	0	0	0	0	0	0	0;
	10	72.3	27.03	300	-300	1.04	100	1	250	10	0	0	0	0	0	0	0	0	0	0	0;
	20	163	6.54	300	-300	1.025	100	1	300	10	0	0	0	0	0	0	0	0	0	0	0;
];

%% branch data
%	fbus	tbus	r	x	b	rateA	rateB	rateC	ratio	angle	status	angmin	angmax
mpc.branch = [
	10	40	0	0.0576	0	250	250	250	0	0	1	-60	60;
	40	50	0.017	0.092	0.158	0	250	250	0	0	1	-60	60;
	50	60	0.039	0.17	0.358	120	150	150	0	0	1	-60	60;
	30	60	0	0.0586	0	0	300	300	0	0	1	-60	60;
	30	60	0	0.0586	0	0	300	300	0	0	0	-60	60;
	60	70	0.0119	0.1008	0.209	120	150	150	0	0	1	-60	60;
	70	80	0.0085	0.072	0.149	0	250	250	0	0	1	-60	60;
	80	20	0	0.0625	0	250	250	250	0	0	1	-60	60;
	80	90	0.032	0.161	0.306	250	250	250	0	0	1	-60	60;
	90	40	0.01	0.085	0.176	250	250	250	0	0	1	-60	60;
];

%%-----  OPF Data  -----%%
%% generator cost data
%	1	startup	shutdown	n	x1	y1	...	xn	yn
%	2	startup	shutdown	n	c(n-1)	...	c0
mpc.gencost = [
	2	3000	0	2	25	0;
	2	2000	0	2	40	0;
	2	1500	0	2	50	0;
	2	2000	0	2	40	0;
];

%%-----  OPF Soft Limit Data  -----%%
%% VMAX soft limit data
mpc.softlims.VMAX.hl_mod = 'none';      %% type of hard limit modification

%% VMIN soft limit data
mpc.softlims.VMIN.hl_mod = 'none';      %% type of hard limit modification

%% PMAX soft limit data
mpc.softlims.PMAX.hl_mod = 'none';      %% type of hard limit modification

%% PMIN soft limit data
mpc.softlims.PMIN.hl_mod = 'none';      %% type of hard limit modification

%% QMAX soft limit data
mpc.softlims.QMAX.hl_mod = 'none';      %% type of hard limit modification

%% QMIN soft limit data
mpc.softlims.QMIN.hl_mod = 'none';      %% type of hard limit modification

%% RATE_A soft limit data
mpc.softlims.RATE_A.hl_mod = 'remove';  %% type of hard limit modification
mpc.softlims.RATE_A.idx = [             %% branch matrix row indices
	2;
	3;
	4;
	5;
	6;
	7;
	8;
	9;
	10;
];
mpc.softlims.RATE_A.cost = [            %% violation cost coefficient
	100;
	100;
	100;
	100;
	100;
	100;
	100;
	100;
	100;
];

%% ANGMAX soft limit data
mpc.softlims.ANGMAX.hl_mod = 'none';    %% type of hard limit modification

%% ANGMIN soft limit data
mpc.softlims.ANGMIN.hl_mod = 'none';    %% type of hard limit modification
