cd src/QuickstartTemplate.Infrastructure
echo "Enter your migration name:"
read msg
dotnet-ef --startup-project ../QuickstartTemplate.WebApi/ migrations add $msg
dotnet ef migrations script -i  --startup-project ../QuickstartTemplate.WebApi/
read


