% correcao de lacos da populacao
function [novosIndividuos, newFxi] = buscaLocalBranchExchange(indBin, alim)

% init vars
newFxi = [];

% chama otimiza ciclo
novosIndividuos = otimizaCicloInd(indBin,alim);

% se criou novos individuos 
if ( ~isempty(novosIndividuos) )

    % introduz todos os individuos na POP ORIGINAL
    % avalia populacao
    newFxi = avaliaPopulacao(novosIndividuos, alim, []); 
   
    % seta status de BuscaLocal do individuo p/ true 
    setStatusBuscaLocalInd(indBin);

    % ordena decrescente
    [newFxi, ind] = sort(newFxi);
    novosIndividuos = novosIndividuos(ind,:);

end

end

% correcao de lacos
% retorna lista de individuos com ciclos 
function lstIndividuos = otimizaCicloInd(indBin,alim)

% transforma binario p/ TS 
indTS = binario2tieSwitch(indBin,alim);

% 
lstIndividuos = otimizaCicloIndividuo(indTS,indBin,alim);

% OLD CODE
% lstIndividuos3 = otmCicloComOutrasChavesFechadas(indTS,indBin,alim);

% transforma p/ binario
lstIndividuos = tieSwitch2binario(lstIndividuos,alim);

% verifica radialidade dos novos individuos
radial = verificaRadialidadeBGL(alim, lstIndividuos);

% retorna somente os individuos radiais
lstIndividuos = lstIndividuos(radial,:);

end

% OBS: NAO USADA.
% correcao de lacos com outras chaves fechadas 
function [lstIndividuos] = otmCicloComOutrasChavesFechadas(indTS,indBinOriginal,alim)

% escolhe ordem de otimizacao das chaves
indTS = defineOrdemDeCiclosOtm(indTS,indBinOriginal,alim);

% vetor que guarda info se TS foi otimizada
otimizouTS(indTS) = 0;

% cria lista de individuos do tamanho do numero de TS (ciclos). 
lstIndividuos = zeros( size(indTS,2),size(indTS,2)  );

% atribui novoIndividuo TS atual 
novoInd = indTS;

% numero de TS
cont=1; 
for TS=indTS

    % fecha todas as chaves NAs
    indBinOriginal(:)=1;

    % abre chave NA
    indBinOriginal(TS)=0;
    
    % se nao otimizou TS 
    if ( ~otimizouTS(TS) )

        % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
        chaveMP = otimizacaoAberturaCiclo(TS, indBinOriginal, alim);
    
    % se ja tiver otimizado pega info no vetor
    else
        
        chaveMP = deParaTS(TS);
        
    end
    
    % se alterou a TS
    if(chaveMP ~= TS)

        % atribui chave ao novo individuo     
        novoInd(cont) = chaveMP;
    
    else
        
        % seta vetor otimizado
        otimizouTS(TS) = 1;
    
    end    

    % lstIndividuos
    lstIndividuos(cont,:) = novoInd;

    cont = cont +1;
end

% filtra lstIndividuos
lstIndividuos = unique(lstIndividuos, 'rows');

end
