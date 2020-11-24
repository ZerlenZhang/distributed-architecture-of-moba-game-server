@echo off  
rem �����ļ� 
for /f "delims=" %%i in ('dir /b/a ".\protos\*.proto"') do echo protos\%%i
rem תcpp  for /f "delims=" %%i in ('dir /b/a "*.proto"') do protoc -I=. --cpp_out=. %%i  
for /f "delims=" %%i in ('dir /b/a ".\protos\*.proto"') do bin\protogen -i:protos\%%i -o:..\Assets\PurificationPioneer\Network\ProgoGen\%%~ni.cs  -ns:PurificationPioneer.Network.ProtoGen
pause  

