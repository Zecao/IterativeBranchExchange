function conf = setaConfiguracoesAG(sistema,algoritmo)

% variavel que armazena se o fluxo ja foi calculado
inicializaHash();

% parametros gerais
global paramAG;

% Codificao Inteira Compact
paramAG.indTSred = 0;

% criacao de populacao cemig
paramAG.metodoGerPopCemig = '2'; % 1 ou 2: mdo 1 cria menos individuos

% indice diversidade genetica
paramAG.limiteIDG = 1.01; 
% booelana que permite 1 migracao
paramAG.migracaoBool = false; %default == true

% indice de individuos radiais
paramAG.IIR = 50; %simulacoes redeIEEE % IIR = 90; %simulacoes rede Cemig
paramAG.IIRMUT = 50;
paramAG.iterIIR = 100; % numero de iteracoes externas p/ criacao IIR.

% tipo de ordenacao dos ciclos
paramAG.tipoOrdCiclos = 'aleatorio'; % 'tamCiclo' 'aleatorio' 'revTamCiclo' ordemClusters 'baseCiclica' 

% parametros DE
global F;
global Cr; 
F = 0.5; % fator de escala mutacao
Cr = 0.1; % constante de cruzamento

global Fm;
global Crm; 
Fm = 0.5; % fator de escala mutacao
Crm = 0.1; % constante de cruzamento

% % booleana de controle de variacao de Cr
% global variaCr;
% variaCr = false;
% 
% % parametros alg genetico classico
% global pMutacao; % probabilidade de mutacao
% pMutacao = 0.05; 
% 
% % numero de bits de codificacao por variavel. 
% % utilizado por funcoes analiticas
% global nBitsCod;
% nBitsCod = 8;

switch sistema
    
    case 0 % alimentadores Cemig
        
        paramAG.numGeracoes = 35;
        
    case 1 % 'alim\Sistema_Zhu2002_3fontes.txt';
        
        paramAG.maxPop = 5;
        paramAG.numGeracoes = 5;
        paramAG.tamPopulacao = 5;

    case 2 % 'Sistema_Zhu2002.txt'; 

        paramAG.maxPop = 10;
        paramAG.numGeracoes = 5;
        paramAG.tamPopulacao = 1; 
        paramAG.tipoOrdCiclos = 'tamCiclo';
        
    case 3 % Sistema_Huang2002.txt';

        paramAG.maxPop = 10;
        paramAG.numGeracoes = 5; 
        paramAG.tamPopulacao = 1;
        paramAG.tipoOrdCiclos = 'tamCiclo';
        
    case 4 % Sistema_Brasileiro2008.txt';

        paramAG.indTSred = 1;
        Crm = 0.15;
        paramAG.numGeracoes = 30;
        paramAG.maxPop = 50; 
        paramAG.tamPopulacao = 10;

    case 5 % Sistema_TPC2003.txt';

        paramAG.tamPopulacao = 1; 
        paramAG.maxPop = 10;
        paramAG.numGeracoes = 5;
        paramAG.tipoOrdCiclos = 'aleatorio';
                paramAG.tipoOrdCiclos = 'tamCiclo';

    case 6 % rede 417barras 
        
        paramAG.tipoOrdCiclos = 'aleatorio';
        paramAG.indTSred = 1;
        paramAG.tamPopulacao = 10;
        paramAG.maxPop = 50;
        paramAG.numGeracoes = 40;
        Crm = 0.15; 
        
    otherwise
        
        conf = [];
end

end

% inicializa hash
function inicializaHash()

global paramAG

paramAG.hashIndividuosCalculados = containers.Map('KeyType', 'char', 'ValueType', 'double');

% verifica se busca local jah foi aplicada ao individuo
paramAG.hashIndividuosOtmBuscaLocal = containers.Map('KeyType', 'char', 'ValueType', 'logical');
paramAG.hashIndividuosOtmBuscaLocalCluster = containers.Map('KeyType', 'char', 'ValueType', 'logical');

end