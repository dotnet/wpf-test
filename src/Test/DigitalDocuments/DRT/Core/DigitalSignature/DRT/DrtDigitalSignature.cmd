@echo off
    call drtfiles\DigitalSignature\CertificateRemove.bat
    call drtfiles\DigitalSignature\CertificateCreate.bat
    DrtDigitalSignature.exe %1
