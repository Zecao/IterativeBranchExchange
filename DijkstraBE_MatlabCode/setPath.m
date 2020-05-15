function setPath()

% get Matlab string path
strPath = path();

path(strPath,'..\BranchExchange');
path(strPath,'..\DifferentialEvolution');
path(strPath,'..\Networks');

% adjust Boost Graph Library path here
path(strPath,'F:\DropboxZecao\Dropbox2018\Dropbox\Dropbox\0 doutorado\mestrado\dissertacao\0soft\matlab_bgl\');

% adjust the Matpowr path here 
path(strPath,'c:\users\ezequiel\Dropbox\0 doutorado\mestrado\dissertacao\0soft\matpower7.0'); 

end
