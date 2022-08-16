% cria matriz sparse mBranch
function branches = criaSparseMBranch(alim)

mBranch = alim.FmBranch;

% seleciona barras Para ativas
branchsAtivos = logical(mBranch(:,11));

% indices das colunas TO e FROM
BUS_TO = 1;
BUS_FROM = 2;

% indice maximo
MAX = max(union(mBranch(:,BUS_TO),mBranch(:, BUS_FROM))); 

% S = sparse(i,j,s,m,n,nzmax) 
branches = sparse(mBranch(branchsAtivos, BUS_TO), mBranch(branchsAtivos, BUS_FROM), 1, MAX, MAX);

% cria grafo bi-direcional
branches = max(branches,branches');

end
