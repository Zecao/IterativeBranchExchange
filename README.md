# Iterative Branch Exchange
Repository for the article [**Distribution Network Reconfiguration using Iterative Branch Exchange and Clustering Techniques**](https://www.mdpi.com/1996-1073/16/5/2395).
As an open-source enthusiast, I decided to publish the source codes for replication of all the results from the article and, as well, future improvements and new methodologies by other researchers. So, this repository is subdivided into the following subdirectories: 

### 1. Article Matlab code
[**"IBE MatlabCode"**](https://github.com/Zecao/IterativeBranchExchange/tree/master/IBE_MatlabCode)
The project uses 2 external programs/libraries 
- Matpower7.0: a free, open-source tool for electric power system simulation and optimization (https://matpower.org/) Run install_matpower.m before.
- Matlab_bgl: a Matlab wrapper for the Boost Graph Library (https://github.com/dgleich/matlab-bgl).

Instructions: put the files in the same root directory of the IBE Matlab code. Before starting, make sure to edit the file setPath.m. So, the following .m files run the Iterative Branch Exchange for each network described in item 1.
- run_16buses.m
- run_33buses.m
- run_70buses.m
- run_84buses.m
- run_136buses.m

1.1 The **results.xlsx** file presents the results of each of the 10 Iterative Branch Exchange runs for the 33, 70, 84, and 136 bus networks.

### 2. Article OpenDSS C# customization code
Essentially, this is a stable version of another repository [**ExecutorOpenDSSBr**](https://github.com/Zecao/ExecutorOpenDssBr). It is a C# (Visual Studio project) OpenDSS customization, one of the features is to run the power flow from real feeders and substations and also perform the Branch Exchange heuristic in the .dss files. The stable version is in a subdirectory (https://github.com/Zecao/IterativeBranchExchange/tree/master/OpenDssCustomization).
After compiling this project in Visual Studio (Community 2022 version), follow the instructions from item 3 below to run the monthly power flow.

### 3. OpenDSS Cemig-D Distribution feeders
[**"CemigFeeders"**](https://github.com/Zecao/IterativeBranchExchange/tree/master/CemigDFeeders)
Directory with the OpenDSS files (.dss) from 2 Cemig distribution feeders and 2 substations. These files are public as they were generated from a geographic distribution database BDGD from the Brazilian Regulatory Agency ANEEL.

**Usage**: 
1. The directory **FeederExample** contains 5 subdirectories: **0PermRes** which contains some resource files such as the *linecode* and the *load profiles* files (I've included these files in a separate subdirectory, as they are usually the same for all feeders). The other four subdirectories () contain the OpenDSS files (.dss) for the 13.8 kV feeder and substation.  
2. You must configure the GUI TextBox "*Caminho dos recursos permanentes*" with the "*Recursos*" subdirectory and the GUI TextBox "*Caminho dos arquivos dos alimentadores \*.dss*" with the root subdirectory.
3. The list of feeders to be executed must be in the file *lstAlimentadores.m*.

Ezequiel C. Pereira
