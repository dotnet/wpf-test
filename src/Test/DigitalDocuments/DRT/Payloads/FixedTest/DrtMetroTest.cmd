@echo off

echo 1. Create the container. 
drtNgcTest /IN drtfiles\Payloads\Sequence\SampleXls.xaml
drtNgcTest /IN drtfiles\NGCTest\constitution.xaml

echo 2. Add the document structure into the container.
DrtAddDocStructure.exe

echo 3. Run hyperlink test. Load container with document structure and run selection test.
DrtFixedTest.exe