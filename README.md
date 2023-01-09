# Iterative Branch Exchange
Repository for the article "Distribution Networks Reconfiguration using an iterative Branch Exchange and metaheuristics".
This repository is subdivided in the following subdirectories: 

### 1. Article Matlab code
[**"IBE MatlabCode"**](https://github.com/Zecao/IterativeBranchExchange/tree/master/IBE_MatlabCode)
(under construction!) I'll publish the Matlab code for replication of all the results from article, **as soon the article be published**.
The project uses 2 external programs/libraries 
- Matpower7.0: a free, open-source tools for electric power system simulation and optimization (https://matpower.org/) Run install_matpower.m before.
- Matlab_bgl: a Matlab wrapper for the Boost Graph Library (https://github.com/dgleich/matlab-bgl).
Instructions: put the files in same root directory of IBE Matlab code. Before start, make sure to edit the file setPath.m. So, the following .m files run the Iterative Branch Exchange for each network described in the item 1.
- run_16buses.m
- run_33buses.m
- run_70buses.m
- run_84buses.m
- run_136buses.m

1.1 The file results.xlsx is an Excel spreadsheet with detailing of the 10 runs of  

### 2. Article OpenDSS C# customization code
Essentially, this is a stable version of another repository [**ExecutorOpenDSSBr**](https://github.com/Zecao/ExecutorOpenDssBr). It is a C# (Visual Studio project) OpenDSS customization, which one of the features is to run the power flow from real feeders and substation and also perform the Branch Exchange heuristic in the .dss files. The stable version is in subdirectory (https://github.com/Zecao/IterativeBranchExchange/tree/master/OpenDssCustomization).
After compiling this project in Visual Studio (Community 2022 version), follow the instructions from item 3 below to run the monthly power flow.

### 3. OpenDSS Cemig-D Distribution feeders
[**"CemigFeeders"**](https://github.com/Zecao/IterativeBranchExchange/tree/master/CemigDFeeders)
Directory with the OpenDSS files (.dss) from 2 Cemig distribution feeders and 2 substations. These files are public as they were generated from a geographic distribution data base BDGD from Brazilian Regulatory Agency ANEEL.

**Usage**: 
1. The directory **FeederExample** contains 5 subdirectories: **0PermRes** which contains some resources files such as the *linecode* and the *load profiles* files (I've included these files in a separated subdirectory, as they are usually the same for all feeders). The other four subdirectories () contains the OpenDSS files (.dss) for the 13.8 kV feeder and subestation.  
2. You must configure the GUI TextBox "*Caminho dos recursos permanentes*" with the "*Recursos*" subdirectory and the GUI TextBox "*Caminho dos arquivos dos alimentadores \*.dss*" with the root subdirectory.
3. The list of feeders to be executed must be in the file *lstAlimentadores.m*.

### 4. Extra material
### 4.1 All reconfigurations  
Spreadsheet detailing all the reconfigurations implemented in [**Cemig-D**](https://www.cemig.com.br/en/) feeders using the Iterative Branch Exchange. 

### 4.2 Initial population for evolutionary meta-heuristics for the Distribution Network Reconfiguration problem
[**"MSTInitialPopulation"**](https://github.com/Zecao/2020Dijkstra/tree/master/MSTInitialPopulation)
Directory with the initial populations for population based evolutionary meta-heuristics representing some configurations for the 33, 70, 84, 136, 417 and 119 buses distribution network. These files are in Matlab format (.mat) and uses the binary codification, i.e. they represent the normally closed (NC) switches by 1s and the normally opened (NO) switches by 0s for the following test cases:  
* (Zhu, 2002) 33 buses system
* (Huang, 2002) 69 (or 70) buses system
* (Su & Lee, 2003) 84 buses system
* (Guimarães & Castro, 2005) 136 buses system
* (Ramirez-rosado & Bernal-Agustín, 1998) 417 (or 415) buses system
* (Zhang, 2007) 119 buses system.

### 5.2 The OPenDSS files (.dss) for 33,70,84,136 buses network.
Also, the OpenDSS files (.dss) for these networks in this directory.

Ezequiel C. Pereira
