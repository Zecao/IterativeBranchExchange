% transforma codificacao binario p/ tie switchs
function newPop = tieSwitch2binario(populacaoTS,alim)

% versao 2.0 (alim como parametro)
if ( ~isempty(alim) )
    
    % traduz chaves cemig
    if ( strcmp(alim.Ftipo,'cemig') )
       
        % TODO; reescrever funcao que elimine esta traducao trabalhando sobre 
        % os branchs manobraveis
        % traduz indices Cemig
        % DELETAR 
        % populacaoTS = indicesMBranch2ChavesIEEECemig(populacaoTS,alim);                
        populacaoTS = chavesIEEE2indicesMBranchCemig(populacaoTS,alim);
        
    end
    
end

% 2019 NEW CODE
% DEFAZ a transformacao de ind TS "reduzido" (que mapeia somente arestas
% que podem ser manobradas, isto, eh arestas que fazem parte do conjunto de 
% ciclos do alimentador) para ind. TS completo.
global paramAG
if (paramAG.indTSred)
    
    % indices TS originais
    indTSori = alim.FmChavesIEEE(:,1);    

    % indices TS manobraveis
    indicesManobraveis = cast(alim.FmChavesIEEE(:,5),'logical');
    indTSmanobraveisDEPARA = indTSori(indicesManobraveis);  

    populacaoTS = traduzPopulacao(populacaoTS,indTSmanobraveisDEPARA);
   
end

% versao 1.0
newPop = tieSwitch2binarioPvt(populacaoTS,alim);

end

% DEFAZ a transformacao de ind TS "reduzido" (que mapeia somente arestas
% que podem ser manobradas, isto, eh arestas que fazem parte do conjunto de 
% ciclos do alimentador) para ind. TS completo.
function newPopTS =  traduzPopulacao(populacaoTS,indTSmanobraveisDEPARA)

newPopTS = [];

for i=1:size(populacaoTS,1)

    individuo = populacaoTS(i,:);
    
    newInd = indTSmanobraveisDEPARA(individuo);
    
    newPopTS = [newPopTS; newInd'];
end

end

function newPop = tieSwitch2binarioPvt(populacao,alim)

% get num vars aliementador
nVars = getNumVarsAlimentador(alim.funcao,alim);

global param;

newPop = ones(size(populacao,1),nVars,param.dataType);

%
for i=1:size(populacao,1)
    
    individuo = populacao(i,:);
    
    % fecha chave na posicao da chave fechada.
    for j=individuo;
    
        % se j == 0, significa que ao verificar-se os lacos, uma chave 
        % pertencia a mais de um laco
        if (j~=0)
            
            newPop(i,j) = 0;
        
        end
            
    end

end

end
