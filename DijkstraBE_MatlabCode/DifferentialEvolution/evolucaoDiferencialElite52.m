% Evolucao Diferencial
function [ populacao, fxi ] = evolucaoDiferencialElite52(alim, populacao, fxi, geracao )

% minimizacao local na primeira geracao
if (geracao == 1)
   
    % minimizacao local na primeira geracao
    [populacao, fxi] = buscaLocal(populacao,fxi,alim);

end

% transforma binario para tie switch
populacao = binario2tieSwitch(populacao,alim); 

% cria Populacao E1
e = criaPopulacaoE(populacao,alim);

% retorna p/ codificacao binaria
populacao = tieSwitch2binario(populacao,alim);

% se criou 1 individuo 
if (size(e,1)>0)
    
    % retorna p/ codificacao binaria
    e = tieSwitch2binario(e,alim);

    %fitness nova populacao
    f_u = avaliaPopulacao(e, alim);
    [f_u, ind ] = sort(f_u);
    e = e(ind,:);
    
    global paramAG;
    
    % % OBS: se comentar esta poda algoritmo LEVE piora (sist=4) 
    lim = ceil(paramAG.tamPopulacao*0.2);

    if ( length(f_u) > lim )
        e = e(1:lim,:);
    end    

    [populacao, fxi] = selecaoUnique(populacao, e, alim);

% OBS: nao da bons resultados rede 4
%     [populacao, fxi] = selecaoDEOriginal(populacao, fxi, e, alim);

end

% busca local
[populacao, fxi] = buscaLocal(populacao,fxi,alim);

end