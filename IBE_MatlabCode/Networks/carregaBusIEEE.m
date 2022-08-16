% %% bus data
 % %	bus_i	type	Pd	Qd	Gs	Bs	area	Vm	Va	baseKV	zone	Vmax	Vmin
 % mpc.bus = [
 % 	1	3	0	0	0	0	1	1	0	345	1	1.1	0.9;
function alim = carregaBusIEEE(opcao,alim)
 
% No 	Tipo 	Vpu	Teta   	Pcarga 	Qcarga  	Pger  	Qger  
switch opcao

    case 1 % 'alim\Sistema_Zhu2002_3fontes.txt';

        A = SistemaZhu20023fontesA();

    case 2 % 'alim\Sistema_Zhu2002';

        A = SistemaZhu2002A();

    case 3 % 'alim\Sistema_Huang2002.txt';

        A = SistemaHuang2002A();

    case 4 % 'alim\Sistema_Brasileiro2008.txt';

        A = Sistema136barras();

    case 42 % 'alim\Sistema_Brasileiro2008_teste';

        A = Sistema136barras_teste();

    case 5 % 'alim\Sistema_TPC2003.txt

        A = SistemaTPC2003A();

    case 6

        A = Sistema417barras();

    case 7

        A = Sistema119barras();
        
    case 8

        A = Sistema703barrasCemig();
        
    otherwise 
        disp('ERRO');

end 
 
% transforma demandas em kva p/ mva 
A(:,5) = kva2mva(A(:,5));
A(:,6) = kva2mva(A(:,6));

% transforma tensao em kv p/ pu
A(:,3) = kv2pu(A(:,3),alim.Vbase);

% converte formato p/ Matpower
 
mBus = zeros(size(A,1),13);
mBus(:,1) = A(:,1); % bus_i
mBus(:,2) = A(:,2); % type
mBus(:,3) = A(:,5); % Pd
mBus(:,4) = A(:,6); % Qd
mBus(:,5) = 0; %1; % Gs conductance
mBus(:,6) = 0; %1; % Bs susceptance
mBus(:,7) = 1; % area
mBus(:,8) = A(:,3); % Vm
mBus(:,9) = A(:,4); % Va
mBus(:,10) = alim.Vbase; % baseKV
mBus(:,11) = 1; % zone
mBus(:,12) = 1.1; % Vmax
mBus(:,13) = 0.8; % Vmin 

alim.FmBus = mBus;

% % TODO refactory. Testando
% %adiciona formato antigo no alim
% alim.FmBarras = A;
 
end
 
function vec = kv2pu(vec,Vbase)
 
vec = vec/Vbase;
 
end
 
 % Table B-1: Bus Data (mpc.bus)
 % name column description
 % BUS I 1 bus number (positive integer)
 % BUS TYPE 2 bus type (1 = PQ, 2 = PV, 3 = ref, 4 = isolated)
 % PD 3 real power demand (MW)
 % QD 4 reactive power demand (MVAr)
 % GS 5 shunt conductance (MW demanded at V = 1.0 p.u.)
 % BS 6 shunt susceptance (MVAr injected at V = 1.0 p.u.)
 % BUS AREA 7 area number (positive integer)
 % VM 8 voltage magnitude (p.u.)
 % VA 9 voltage angle (degrees)
 % BASE KV 10 base voltage (kV)
 % ZONE 11 loss zone (positive integer)
 % VMAX 12 maximum voltage magnitude (p.u.)
 % VMIN 13 minimum voltage magnitude (p.u.)
 % LAM P� 14 Lagrange multiplier on real power mismatch (u/MW)
 % LAM Q� 15 Lagrange multiplier on reactive power mismatch (u/MVAr)
 % MU VMAX� 16 Kuhn-Tucker multiplier on upper voltage limit (u/p.u.)
 % MU VMIN� 17 Kuhn-Tucker multiplier on lower voltage limit (u/p.u.)
 