% cria matriz esparsa(map)
function alim = criaMapVerticesArestas(alim)

mBranch = alim.FmBranch;

% indices das colunas TO e FROM
BUS_TO = 1;
BUS_FROM = 2;

% indice maximo
MAX = max(union(mBranch(:,BUS_TO),mBranch(:, BUS_FROM))); 

% vetor sequencial de 1 ao tamanho de linhas de mBranch representando as arestas
arestas = [1:1:size(mBranch,1)]';

% S = sparse(i,j,s,m,n,nzmax) 
map = sparse(mBranch(:, BUS_TO), mBranch(:, BUS_FROM), arestas, MAX, MAX);

% cria grafo bi-direcional
map = max(map,map');

% adioiona
alim.FmapVerticesArestas = map;

end