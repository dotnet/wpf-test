# Different Test Suites and Areas

## Categorization on the basis of suites
The tests in this repo belong to one of these main categories : 

- **DRT ( Daily Regression Tests)** : This is a small suite of tests, that are meant to cover the most basic scenarios of different components of WPF. The general idea, regarding this test suite is to provide a basic validation that the changes are not causing regressions. There are around 90 DRTs, and it takes roughly 20-40 mins to run the whole suite.
   
- **Feature Tests** : They are the bulk of all the WPF tests. They include various scenarios and are meant for testing each component ( area ) more rigorously. 
  - **Microsuites** : They are a very small subset of the feature tests and are intended to be run along with all the standard fixes. 
  - **P0/P1/..** : All the tests in the suite have a priority ( 0,1,2,3,4 ) assigned to them. P0 ( i.e. Priority 0 ) are the most important of the feature tests. Then comes P1, P2 and so on. 


## Categorization on the basis of area
Another categorization of the tests in this repo is on the basis of component ( area ) of WPF the tests target. Here is a list of all the areas:
- 2D
- 3D
- AdvancedInput
- Animation
- Annotations
- AppModel
- Controls
- Diagnostics
- DataServices
- DigitalDocuments
- Editing
- ElementLayout
- ElementServices
- FlowLayout
- Globalization
- Graphics
- Imaging
- Media
- Printing
- Speech
- Tablet
- Text
- XAML ( XamlV3 and XamlV4 )
- WindowsFormsIntegration

PS: *You can navigate to each area and see the subareas*
Each area is further subdivided into subareas. You can see the list of subareas by visiting area-specific file from the above links. Using both Area and SubArea, will allow you to run a subset of tests that are related to the changes you are making 