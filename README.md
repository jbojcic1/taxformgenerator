# taxformgenerator
Console app for generating Croatian tax forms. You can pull code and build it or you can get [PREBUILT VERSION HERE](https://drive.google.com/open?id=1Uvm1_zcpD27JXNO1OQyLcKvf6TjM8_NU)

REQUIREMENTS: 
  - macos release >= 10.12
  - dotnet-sdk

If you don't have dotnet-sdk installed:
1. Execute: "brew cask install dotnet-sdk"
2. Quit and reopen terminal. Execute "dotnet --info" to see if installation was successfull. 
   If not execute "ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/" and do "dotnet --info" again.


BUILD APP (skip if you are using prebuilt version):
  - Execute "dotnet publish -c Release -r osx.10.11-x64"


USAGE:
  - Navigate to "TaxFormGenerator/bin/Release/netcoreapp2.0/osx.10.11-x64/publish" or to you app folder if you have prebuilt version
  - Modify following config files with your params:
      * "FormGenerator/SalaryJOPPD/ContributionsJOPPDTemplate.xml"
      * "FormGenerator/SalaryJOPPD/ContributionsJOPPDTemplate.xml"
      * "SalaryCalculator/SalaryConfig.json"
  - Run "dotnet TaxFormGenerator.dll {PARAMS}"
  - XML forms will be generated in "Output" folder



PARAMS:
  1) FormType
    - type of the form
    - default value: SalaryJOPPD
    - possible values: SalaryJOPPD (In this version this is only possible value. Later there will be also DividendJOPPD, etc.)
  2) Date
    - data when form was generated
    - default value: now
    - format: MM/DD/YYYY
  3) Amount
    - payment amount
    - default value: 830
  4) Currency
    - payment currency
    - default value: "EUR"
  5) SalaryMonth
    - month of the salary for which form is generated
    - default value: month before the Date param
    - format: MM/YYYY

Example of the run:
  dotnet ./TaxFormGenerator.dll --date 12/21/2017 --salaryMonth 11/2017

    
