@echo off
@echo Creating Certificates
makecert.exe -n "CN=Continents, O=World, L=Earth, C=All" -ss "Root" -a sha1 -len 2048 -r -cy authority -sr "LocalMachine" -sk "D8C0990E-1B94-43cc-B37B-1C0FFFB5E3B4"
makecert.exe -n "CN=Africa" -len 2048 -a sha1 -cy end -is "Root" -in "Continents" -ir "LocalMachine" -ss "My" -sr "CurrentUser" .\drtfiles\DigitalSignature\Africa.cer -sk "D8C0990E-1B94-43cc-B37B-1C0FFFB5E3B4" > nul
makecert.exe -n "CN=Australia" -len 2048 -a sha1 -cy end -is "Root" -in "Continents" -ir "LocalMachine" -ss "My" -sr "CurrentUser" .\drtfiles\DigitalSignature\Australia.cer -sk "D8C0990E-1B94-43cc-B37B-1C0FFFB5E3B4" > nul
makecert.exe -n "CN=Asia" -len 2048 -a sha1 -cy end -is "Root" -in "Continents" -ir "LocalMachine" -ss "My" -sr "CurrentUser" .\drtfiles\DigitalSignature\Asia.cer -sk "D8C0990E-1B94-43cc-B37B-1C0FFFB5E3B4" > nul
