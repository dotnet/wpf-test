@echo off
@echo Removing Certificates
delcertNoUI "CurrentUser" "My" "Africa"
delcertNoUI "CurrentUser" "My" "Australia"
delcertNoUI "CurrentUser" "My" "Asia"
delcertNoUI "LocalMachine" "Root" "Continents"

@if EXIST .\drtfiles\DigitalSignature\Africa.cer del /f .\drtfiles\DigitalSignature\Africa.cer > nul
@if EXIST .\drtfiles\DigitalSignature\Australia.cer del /f .\drtfiles\DigitalSignature\Australia.cer > nul
@if EXIST .\drtfiles\DigitalSignature\Asia.cer del /f .\drtfiles\DigitalSignature\Asia.cer > nul

