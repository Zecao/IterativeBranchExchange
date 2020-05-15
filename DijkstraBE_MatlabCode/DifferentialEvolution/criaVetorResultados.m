% cria vetor resultados 
function vetorResultados = criaVetorResultados(algoritmo,arrayStructElite,telapsed,numMigracoes)

% global paramAG;
global param;

% obtem vetor perdas(elite) por geracao  
perdasXGen = getPerdasXGen(arrayStructElite);

% cria vetorResultados 
vetorResultados = [
    % algoritmo;
    perdasXGen;
    param.NCAL;
    telapsed;
%     param.NOAC;
%     numMigracoes;
    param.NCALBL;
    param.SucessoSEL; % param.nSucBIJ
    ];

end