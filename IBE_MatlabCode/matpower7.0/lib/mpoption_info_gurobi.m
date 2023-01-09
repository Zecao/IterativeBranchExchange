function opt = mpoption_info_gurobi(selector)
%MPOPTION_INFO_GUROBI  Returns MATPOWER option info for Gurobi.
%
%   DEFAULT_OPTS = MPOPTION_INFO_GUROBI('D')
%   VALID_OPTS   = MPOPTION_INFO_GUROBI('V')
%   EXCEPTIONS   = MPOPTION_INFO_GUROBI('E')
%
%   Returns a structure for Gurobi options for MATPOWER containing ...
%   (1) default options,
%   (2) valid options, or
%   (3) NESTED_STRUCT_COPY exceptions for setting options
%   ... depending on the value of the input argument.
%
%   This function is used by MPOPTION to set default options, check validity
%   of option names or modify option setting/copying behavior for this
%   subset of optional MATPOWER options.
%
%   See also MPOPTION.

%   MATPOWER
%   Copyright (c) 2014-2016, Power Systems Engineering Research Center (PSERC)
%   by Ray Zimmerman, PSERC Cornell
%
%   This file is part of MATPOWER.
%   Covered by the 3-clause BSD License (see LICENSE file for details).
%   See https://matpower.org for more info.

if nargin < 1
    selector = 'D';
end
if have_fcn('gurobi')
    switch upper(selector)
        case {'D', 'V'}     %% default and valid options
            opt = struct(...
                'gurobi',       struct(...
                    'method',       -1, ...
                    'timelimit',    Inf, ...
                    'threads',      0, ...
                    'opts',         [], ...
                    'opt_fname',    '', ...
                    'opt',          0 ...
                ) ...
            );
        case 'E'            %% exceptions used by nested_struct_copy() for applying
            opt = struct(...
                'name',         { 'gurobi.opts' }, ...
                'check',        0 ...
                );
%                 'copy_mode',    { @gurobi_options } ...
        otherwise
            error('mpoption_info_gurobi: ''%s'' is not a valid input argument', selector);
    end
else
    opt = struct([]);       %% Gurobi is not available
end
