
% plota informacoes do algoritmo genetico no arquivo txt
function plotaInfoAG2( algoritmo, sistema, numexec, matrizResultados )

% salva arquivo
FILENAME = strcat('InfoAG',num2str(algoritmo),'_',num2str(sistema),'_',num2str(numexec),'.txt');
save(FILENAME,'matrizResultados','-ascii','-tabs');

end
