cd ..\N_m3u8DL-RE
dotnet build -c release -o %1
md ..\DLStart\%1core
copy %1*.* ..\DLStart\%1core\
