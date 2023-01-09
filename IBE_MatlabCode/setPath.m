function setPath()

% strPath = '';

path(path,pwd);
path(path,strcat(pwd,'\BranchExchange'));
path(path,strcat(pwd,'\DifferentialEvolution'));
path(path,strcat(pwd,'\matlab_bgl'));
path(path,strcat(pwd,'\matpower7.0'));
path(path,strcat(pwd,'\Networks'));

end 