# Test Infrastructure

The test infra consists mainly of the following runtime assemblies : `TestRuntime.dll`, `TestContracts.dll`, `InternalUtilities.dll`, `QualityVaultFrontEnd.exe`, `QualityVaultUtilities.dll`

`InternalUtilities.dll` is responsible for Log manager and Log Interface.

`TestRuntime.dll` and `TestContracts.dll` have the basic classes that describe the tests like : TestAttribute, TestInfo, TestRecord, VariationAttribute, VariationRecord, Discovery Adaptor, list and object serializers, etc.

QualityVault ( `QualityVaultFrontEnd.exe` and `QualityVaultUtilities.dll`) is the test harness that is responsible for launching the run, generating reports. It includes various components like - Discovery Engine ( finds all the tests in the code ), Filtering Engine ( filters out the tests that needs to be ignored ), Test Adaptors ( define the way in which and how the tests are to be discovered ), Test Collection and Test Executor ( responsible for launching the tests ). Quality Vault also has `QualityVaultDebugger.exe` which introduces debugging capability.

Apart from test infra runtime assemblies, we have a test driver `Sti.exe` which is responsible for launching each test.

### Test Adaptors
There are different test adaptors that define the way in which the tests will be discovered.
- AnnotationAdaptor 
- TestAttributeAdaptor 
- **XtcAdaptor** : Looks for all the XTC files in a path, and processes these files to find tests and their support files. XTC files also define test driver and other information required to run the tests.  
- VariationAttributeAdaptor 
- **DrtAdaptor** : This adaptor takes in a file argument ( in `DiscoveryInfoDrts.xml` the file is `DrtList.xml` ) and reads it to find the tests and support files needed to run each test.

Depending on the adaptor a test area is using, the new test addition guidelines will be different.

### Test Discovery

Test harness uses [`DiscoveryInfo.xml`]() and [`DiscoveryInfoDrts.xml`]() when we run `RunTests.cmd` and `RunDrts.cmd` to find feature tests and DRTs respectively.

`DiscoveryInfoDrts.xml` then reads the list of DRTs from [`DrtList.xml`]()