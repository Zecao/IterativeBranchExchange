% carrega branch
function alim = carregaBranchIEEE(sistema,alim)
 
switch sistema 
     
    case 1 % 16 buses network (Zhu2002)

        B = SistemaZhu20023fontesB();

    case 2 % 33 buses network (Zhu2002)

        B = SistemaZhu2002B();

    case 3 % 70 buses network (Huang2002)

        B = SistemaHuang2002B();
        
    case 31 % 70 buses network (Huang2002) 1 ciclo

        B = SistemaHuang2002B_31();

    case 4 % 136 buses network (Brazilian)

        B = Sistema136branchs();

    case 5 % 84 buses network (TPC)

        B = SistemaTPC2003B();         

    case 6 % 417 buses network

        B = Sistema417branchs();

    case 7 % 119 buses network

        B = Sistema119branchs();
        
    case 8 % 703 buses network

        B = Sistema703branchsCemig();
        
    otherwise 
        disp('ERRO');
                 
end 
 
% transforma base, se necessario
B = zohms2zpu(B,sistema,alim.Sbase,alim.Vbase);

% TODO testando
% %adiciona formato antigo no alim
% % De Para   R(pu)   X(pu)   Comprimento(km)   Ampacidade(A)
% alim.FmLinhas = B;

% preenceh mBranch
mBranch = zeros(size(B,1),13);

mBranch(:,1) = B(:,1); %
mBranch(:,2) = B(:,2); %
mBranch(:,3) = B(:,3).*B(:,5); % OBS resistencia em PU/km. OK
mBranch(:,4) = B(:,4).*B(:,5);
mBranch(:,5) = 0; % b 
mBranch(:,6) = B(:,6); % rateA
mBranch(:,7) = B(:,6); % rateB
mBranch(:,8) = B(:,6); % rateC
mBranch(:,9) = 1; % ratio
mBranch(:,10) = 0; % angle
mBranch(:,11) = 1; % status
mBranch(:,12) = -360; % angmin
mBranch(:,13) = 360; % angmax

alim.FmBranch = mBranch;
 
end
 
 % transforma Z(ohms) para Zpu, se necessario 
 function B = zohms2zpu(B,sistema,Sbase,Vbase)
 
% OBS: todos os sistemas a excecao do 1 e 8 devem ter impedancias
% trasnformadas p/ pu
if ( ~ ( (sistema == 1) )) %||(sistema ==8) ) )
    
     % calculo zBase
     zBase = Vbase^2/Sbase; 
 
     % calculo zpu = zohms/zbase;
     B(:,3) = B(:,3)/zBase; 
     B(:,4) = B(:,4)/zBase; 
 
 end
 
 end
 
 % %% branch data
 % %	fbus	tbus	r	x	b	rateA	rateB	rateC	ratio	angle	status	angmin	angmax
 % mpc.branch = [
 % 	1	4	0	0.0576	0	250	250	250	0	0	1	-360	360;
 % 	4	5	0.017	0.092	0.158	250	250	250	0	0	1	-360	360;
 % 	5	6	0.039	0.17	0.358	150	150	150	0	0	1	-360	360;
 % 	3	6	0	0.0586	0	300	300	300	0	0	1	-360	360;
 % 	6	7	0.0119	0.1008	0.209	150	150	150	0	0	1	-360	360;
 % 	7	8	0.0085	0.072	0.149	250	250	250	0	0	1	-360	360;
 % 	8	2	0	0.0625	0	250	250	250	0	0	1	-360	360;
 % 	8	9	0.032	0.161	0.306	250	250	250	0	0	1	-360	360;
 % 	9	4	0.01	0.085	0.176	250	250	250	0	0	1	-360	360;
 % ];
 
 % % Table B-3: Branch Data (mpc.branch)
 % % name column description
 % % F BUS 1 \from" bus number
 % % T BUS 2 \to" bus number
 % % BR R 3 resistance (p.u.)
 % % BR X 4 reactance (p.u.)
 % % BR B 5 total line charging susceptance (p.u.)
 % % RATE A 6 MVA rating A (long term rating)
 % % RATE B 7 MVA rating B (short term rating)
 % % RATE C 8 MVA rating C (emergency rating)
 % % TAP 9 transformer o nominal turns ratio, (taps at \from" bus,
 % % impedance at \to" bus, i.e. if r = x = 0, tap = jVf j
 % % jVtj )
 % % SHIFT 10 transformer phase shift angle (degrees), positive ) delay
 % % BR STATUS 11 initial branch status, 1 = in-service, 0 = out-of-service
 % % ANGMIN* 12 minimum angle diference, f ? t (degrees)
 % % ANGMAX* 13 maximum angle diference, f ? t (degrees)
 % % PF† 14 real power injected at \from" bus end (MW)
 % % QF† 15 reactive power injected at \from" bus end (MVAr)
 % % PT† 16 real power injected at \to" bus end (MW)
 % % QT† 17 reactive power injected at \to" bus end (MVAr)
 % % MU SF‡ 18 Kuhn-Tucker multiplier on MVA limit at \from" bus (u/MVA)
 % % MU ST‡ 19 Kuhn-Tucker multiplier on MVA limit at \to" bus (u/MVA)
 % % MU ANGMIN‡ 20 Kuhn-Tucker multiplier lower angle diference limit (u/degree)
 % % MU ANGMAX‡ 21 Kuhn-Tucker multiplier upper angle diference limit (u/degree)
 % % * Not included in version 1 case format.
 % % † Included in power 
 % % ow and OPF output, ignored on input.
 % % ‡ Included in OPF output, typically 