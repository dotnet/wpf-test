# Different Test Suites and Areas

The tests in this repo belong to one of these main categories : 

- **DRT ( Daily Regression Tests)** : This is a small suite of tests, that are meant to cover the most basic scenarios of different components of WPF. The general idea, regarding this test suite is to provide a basic validation that the changes are not causing regressions. There are around 90 DRTs, and it takes roughly 20-40 mins to run the whole suite.
   
- **Feature Tests**
  - **Microsuites**
  - **P0/P1/..** 


Another categorization of the tests in this repo is on the basis of componenent ( area ) of WPF the tests target. Here is a list of all the areas:
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
- XAML
- WindowsFormsIntegration

Each area is further subdivided into subareas. You can see the list of subareas by visiting area-specific file from the above links. Using both Area and SubArea, will allow you to run a subset of tests that are related to the changes you are making 