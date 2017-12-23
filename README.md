# taxformgenerator
Console app for generating Croatian tax forms. You can pull code and build it or you can get [PREBUILT VERSION HERE](https://github.com/jbojcic1/taxformgenerator/releases/download/v1.0.0/OSx.-.Tax.Form.Generator.zip)

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
    - possible values: 
        * SalaryJOPPD
        * DividendJOPPD
    - default value: SalaryJOPPD
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
  6) StartDate
    - start date of the period for which dividend is being paid
    - default value: 1st January of the year specified with Date param
    - format: MM/DD/YYYY
  7) EndDate
    - end date of the period for which dividend is being paid
    - default value: 31st December of the year specified with Date param
    - format: MM/DD/YYYY

Example of the run:
  1) salary JOPPD:    dotnet ./TaxFormGenerator.dll --date 12/19/2017 --salaryMonth 11/2017
  2) dividend JOPPD:  dotnet ./TaxFormGenerator.dll --formType DividendJOPPD --date 12/20/2017 --amount 5000

    
