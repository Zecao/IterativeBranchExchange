% correcao de lacos da populacao
function [novosIndividuos, newFxi] = buscaLocalBranchExchange(individuo, alim)

% init vars
newFxi = [];

% chama otimiza ciclo
novosIndividuos = otimizaCicloIndividuo(individuo,alim);

% se criou novos individuos 
if ( ~isempty(novosIndividuos) )

    % introduz todos os individuos na POP ORIGINAL
    % avalia populacao
    newFxi = avaliaPopulacao(novosIndividuos, alim, []); 
   
% % % TODO testar se eh melhor que o set na funcao "adiciona"
    % seta status de BuscaLocal do individuo p/ true 
    setStatusBuscaLocalInd(individuo);

    % ordena decrescente
    [newFxi, ind] = sort(newFxi);
    novosIndividuos = novosIndividuos(ind,:);

end

end

% correcao de lacos
% retorna lista de individuos com ciclos 
function lstIndividuos = otimizaCicloIndividuo(indBin,alim)

% transforma binario p/ TS 
indTS = binario2tieSwitch(indBin,alim);

% sistema
sis = getSistema(alim.Fnome);

% 
% global paramAG

%OBS: se nao usar otimizaCicloIndividuoPrivate (BE antiga) tenho baixa div.
% populacional nas redes 4 e 6.
switch (sis)
     
    case 4 

% %       % OBS: melhores resultados 
        lstIndividuos = otimizaCicloIndividuoPrivate2020(indTS,indBin,alim);
        
% OBS: piora um pouco se usar otimizaCicloIndividuoPrivate() e muito com 
% otmCicloComOutrasChavesFechadas() 
        
%         % DEBUG. OBS: nao da bons resultados 
%         paramAG.tipoOrdCiclos = 'tamCiclo';
%         lstIndividuos1 = otimizaCicloIndividuoPrivate(indTS,indBin,alim);   
%         
%         lstIndividuos2 = otimizaCicloIndividuoPrivate2020(indTS,indBin,alim);
% 
%         paramAG.tipoOrdCiclos = 'aleatorio';        
%         lstIndividuos3 = otimizaCicloIndividuoPrivate(indTS,indBin,alim);
%         
%         lstIndividuos4 = otimizaCicloIndividuoPrivate2020(indTS,indBin,alim);
%      
%         lstIndividuos = [lstIndividuos1; lstIndividuos2; lstIndividuos3; lstIndividuos4]; 
    
    case 6 

        lstIndividuos1 = otimizaCicloIndividuoPrivate2020(indTS,indBin,alim);

        % corrige Lacos
        lstIndividuos2 = otimizaCicloIndividuoPrivate(indTS,indBin,alim);

        lstIndividuos3 = otmCicloComOutrasChavesFechadas(indTS,indBin,alim);

        lstIndividuos = [lstIndividuos1; lstIndividuos2;lstIndividuos3]; 
    
    otherwise       
    
        % OBS: nao da bons resultados rede 4
        lstIndividuos = otimizaCicloIndividuoPrivate2020(indTS,indBin,alim);
    
end

% transforma p/ binario
lstIndividuos = tieSwitch2binario(lstIndividuos,alim);

% verifica radialidade dos novos individuos
radial = verificaRadialidadeBGL(alim, lstIndividuos);

% retorna somente os individuos radiais
lstIndividuos = lstIndividuos(radial,:);

end

% correcao de lacos
function lstIndividuos = otimizaCicloIndividuoPrivate(indTS,indBinOriginal,alim)

% escolhe ordem de otimizacao das chaves
indTS = defineOrdemDeCiclosOtm(indTS,indBinOriginal,alim);

% novo individuo inicialmente eh igual as Tie Switchs antigas. Ele vai
% sendo preenchido chave por chave.
novoIndividuo = indTS;

% cria lista de individuos. Cada individuo representara um passo (loop de
% for) da transicao do indTS p/ o novoIndividuo.
lstIndividuos = zeros( size(indTS,2),size(indTS,2)  );

% para cada chave, i, do individuo
for i=1:size(indTS,2); 

    % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
    chaveMP = otimizacaoAberturaCiclo(indTS(i), indBinOriginal, alim);
    
    % se chave menor perda jah esta no individuo, mantem chave anterior
    if ( ismember(chaveMP, novoIndividuo) )
    
        % mantem chave anterior
        novoIndividuo(i) = indTS(i); 

    else
        
        % preenche novo individuo     
        novoIndividuo(i) = chaveMP; 

    end
    
    lstIndividuos(i,:) = novoIndividuo;

    % incrementa contador OAC
    contaOAC();
    
end

% filtra lstIndividuos
lstIndividuos = unique(lstIndividuos, 'rows');

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

deParaTS = [];

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
    
    % OBS: nao tem uso/ganho eficiencia. Mantendo o codigo, como lembrete.
    deParaTS(TS) = chaveMP;

    % lstIndividuos
    lstIndividuos(cont,:) = novoInd;

    cont = cont +1;
end

% incrementa contador OAC
contaOAC();

% filtra lstIndividuos
lstIndividuos = unique(lstIndividuos, 'rows');

end


% correcao de lacos
function [lstIndividuos] = otimizaCicloIndividuoPrivate2020(indTS,indBinOriginal,alim)

% escolhe ordem de otimizacao das chaves
indTS = defineOrdemDeCiclosOtm(indTS,indBinOriginal,alim);

% vetor que guarda info se TS foi otimizada
otimizouTS(indTS) = 0;

% cria lista de individuos do tamanho do numero de TS (ciclos). 
lstIndividuos = zeros( size(indTS,2),size(indTS,2)  );

% atribui novoIndividuo TS atual 
novoInd = indTS;

deParaTS = [];

% numero de TS 
for i=1:size(indTS,2)

%     % debug 
%     if ( indTS(i)==38 || indTS(i)==53 )
% 
%     indBinBAK = indBinOriginal
%     indBinOriginal(:)=1;
%     indBinOriginal(53)=0;
% 
% % indTSnew = [35,53]; % se otimizar nessa ordem(maior laço), a chave 35->38 (otm. local)
% % indTSnew = [53,35]; % se otimizar nessa ordem(menor laço), chave 53->142 (otm. global)
% 
% % situacao1. otimizando chave 53 (mais externa)
% % indBinOriginal(38)=1 % altera chave 38
% % indBinOriginal(35)=0 -> chega na chave 142
% 
% % situacao1. otimizando chave 38 (interna)
% % indBinOriginal(53)=1; % altera chave 53 OBS: com 53=0 (aberta) nao chega na chave
% % % indBinOriginal(142)=0; % teste1 -> chega na chave 35
% % % indBinOriginal(54)=0; % teste2 -> chega na chave 35
% % indBinOriginal(55)=0; % teste3 -> chega na chave 35
% 
%     debug=0;
% 
%     end
    
    % se nao otimizou TS 
    if ( ~otimizouTS(indTS(i)) )

        % obtem a chave da condicao de menor perda p/ dada chave indTS(i)
        chaveMP = otimizacaoAberturaCiclo(indTS(i), indBinOriginal, alim);
    
    % se ja tiver otimizado pega info no vetor
    else
        
        chaveMP = deParaTS(indTS(i));
        
    end
    
    % se alterou a TS
    if(chaveMP ~= indTS(i))
        
        % altera indBin (para analise da proximo ciclo)
        indBinOriginal(indTS(i)) = 1;
        indBinOriginal(chaveMP) = 0;
        
        % atribui chave ao novo individuo     
        novoInd(i) = chaveMP;
    
    else
        
        % seta vetor otimizado
        otimizouTS(indTS(i)) = 1;
    
    end
    
    % OBS: nao tem uso/ganho eficiencia. Mantendo o codigo, como lembrete.
    deParaTS(indTS(i)) = chaveMP;

    % lstIndividuos
    lstIndividuos(i,:) = novoInd;

end

% % % OBS: nao faz sentido marcar o individuo como avalidado, devido a natureza estocastica
% % % da Branch Exchange em sistemas com um ciclo dentro do outro.
% sis = getSistema(alim.Fnome);
% 
% if ( ~(sis==4) || (sis==6) )  
% 
% % OBS: ('Busca Local esgotada!');
% % se todas as chavesMP == indTS(i)
% if (all(otimizouTS(indTS)))
%     
%     disp('Busca Local esgotada p/ este IND!');
% 
%     % seta status de BuscaLocal do individuo p/ true 
%     setStatusBuscaLocalInd(indBinOriginal);
%     
%     lstIndividuos = [];
%     
%     return
% 
% end
% 
% end

% incrementa contador OAC
contaOAC();

% filtra lstIndividuos
lstIndividuos = unique(lstIndividuos, 'rows');

end

% incrementa contador OAC
function contaOAC()

global param;
param.NOAC = param.NOAC + 1;

end