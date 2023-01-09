function varargout = runsdpopf(casedata, mpopt, fname, solvedcase)
%RUNSDPOPF  Runs a semidefinite relaxation of the AC optimal power flow.
%   [RESULTS, SUCCESS] = RUNSDPOPF(CASEDATA, MPOPT, FNAME, SOLVEDCASE)
%
%   Runs a semidefinite programming relaxation of the AC optimal power flow, 
%   optionally returning a RESULTS struct and SUCCESS flag.
%
%   Inputs (all are optional):
%       CASEDATA : either a MATPOWER case struct or a string containing
%           the name of the file with the case data (default is 'case9')
%           (see also CASEFORMAT and LOADCASE)
%       MPOPT : MATPOWER options struct to override default options
%           can be used to specify the solution algorithm, output options
%           termination tolerances, and more (see also MPOPTION).
%       FNAME : name of a file to which the pretty-printed output will
%           be appended
%       SOLVEDCASE : name of file to which the solved case will be saved
%           in MATPOWER case format (M-file will be assumed unless the
%           specified name ends with '.mat')
%
%   Outputs (all are optional):
%       RESULTS : results struct, with the following fields:
%           (all fields from the input MATPOWER case, i.e. bus, branch,
%               gen, etc., but with solved voltages, power flows, etc.)
%           order - info used in external <-> internal data conversion
%           et - elapsed time in seconds
%           success - success flag, 1 = succeeded, 0 = failed
%           (additional OPF fields, see OPF for details)
%       SUCCESS : the success flag can additionally be returned as
%           a second output argument
%
%   Calling syntax options:
%       results = runsdpopf;
%       results = runsdpopf(casedata);
%       results = runsdpopf(casedata, mpopt);
%       results = runsdpopf(casedata, mpopt, fname);
%       results = runsdpopf(casedata, mpopt, fname, solvedcase);
%       [results, success] = runsdpopf(...);
%
%       Alternatively, for compatibility with previous versions of MATPOWER,
%       some of the results can be returned as individual output arguments:
%
%       [baseMVA, bus, gen, gencost, branch, f, success, et] = runsdpopf(...);
%
%   Example:
%       results = runsdpopf('case30');
%
%   See also RUNOPF.

%   MATPOWER
%   Copyright (c) 1996-2019, Power Systems Engineering Research Center (PSERC)
%   by Ray Zimmerman, PSERC Cornell
%   and Daniel Molzahn, PSERC U of Wisc, Madison
%
%   This file is part of MATPOWER/mx-sdp_pf.
%   Covered by the 3-clause BSD License (see LICENSE file for details).
%   See https://github.com/MATPOWER/mx-sdp_pf/ for more info.

%% default arguments
if nargin < 4
    solvedcase = '';                %% don't save solved case
    if nargin < 3
        fname = '';                 %% don't print results to a file
        if nargin < 2
            mpopt = mpoption;       %% use default options
            if nargin < 1
                casedata = 'case9'; %% default data file is 'case9.m'
            end
        end
    end
end

mpopt = mpoption(mpopt, 'opf.ac.solver', 'SDPOPF');
[varargout{1:nargout}] = runopf(casedata, mpopt, fname, solvedcase);
