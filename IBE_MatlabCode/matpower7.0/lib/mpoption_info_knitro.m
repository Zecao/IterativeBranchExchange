function opt = mpoption_info_knitro(selector)
%MPOPTION_INFO_KNITRO  Returns MATPOWER option info for Artelys Knitro.
%
%   DEFAULT_OPTS = MPOPTION_INFO_KNITRO('D')
%   VALID_OPTS   = MPOPTION_INFO_KNITRO('V')
%   EXCEPTIONS   = MPOPTION_INFO_KNITRO('E')
%
%   Returns a structure for Knitro options for MATPOWER containing ...
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
if have_fcn('knitro')
    switch upper(selector)
        case {'D', 'V'}     %% default and valid options
            opt = struct(...
                'knitro',       struct(...
                    'tol_x',        1e-4, ...
                    'tol_f',        1e-4, ...
                    'maxit',        0, ...
                    'opts',         [], ...
                    'opt_fname',    '', ...
                    'opt',          0 ...
                ) ...
            );
        case 'E'            %% exceptions used by nested_struct_copy() for applying
            opt = struct(...
                'name',         { 'knitro.opts' }, ...
                'check',        0, ...
                'copy_mode',    { '' } ...
                );
        otherwise
            error('mpoption_info_knitro: ''%s'' is not a valid input argument', selector);
    end
else
    opt = struct([]);       %% Artelys Knitro is not available
end
