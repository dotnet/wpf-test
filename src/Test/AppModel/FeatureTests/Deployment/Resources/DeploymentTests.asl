<AppSecLoaderConfigs>
    <CommonConfig>
        <FeatureTeam>Application Model</FeatureTeam>
        <FeatureArea>Application</FeatureArea>        
    </CommonConfig>

<!-- Basic Scenarios -->
  <!-- SEE Xbap -->
    <AppSecLoaderConfig ID="1">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch and verify a basic Internet-Zone Browser-Hosted app (.xbap) over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="amc_deployment_internetxbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="2">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Navigate using IE address bar to the same .xbap from Scenario #1 over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_NavigateInternetXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

  <!-- Intranet zone Xbap -->
    <AppSecLoaderConfig ID="3">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch and verify a basic Intranet-Zone Browser-Hosted app (.xbap) over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <targetzone name="LocalIntranet" />
           <application name="ExpressAppIntranetZone" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="amc_deployment_intranetxbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="4">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Navigate using IE address bar to the same .xbap from Scenario #3 over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <targetzone name="LocalIntranet" />
           <application name="ExpressAppIntranetZone" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_NavigateIntranetXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

  <!-- SEE Xbap -->
    <AppSecLoaderConfig ID="5">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch and verify a basic full-trust Browser-Hosted app (.xbap) over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleBrowserHostedNSVApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="amc_deployment_FullTrustxbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="6">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Navigate using IE address bar to the same .xbap from Scenario #5 over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleBrowserHostedNSVApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_NavigateFullTrustXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>
  <!-- FT .Application -->
    <AppSecLoaderConfig ID="7">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch and verify a simple .application over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_BasicApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="8">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>0</Priority>        
        <Description>Navigate IE to a simple .application over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_NavigateBasicApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>
 <!-- End Basic Scenarios -->
 
 <!-- WebOC Scenarios -->
    <AppSecLoaderConfig ID="9">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch and verify an Xbap that loads HTML in a WebOC control, over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <application name="ExpressAppWebOC" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_WebOCXbapMarkup.xaml" />
           <resource filename="deploy_htmlmarkup.htm" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="amc_deployment_WebOCXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
 
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="10">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch and verify a standalone app that loads HTML in a WebOC control, over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <application name="StandaloneAppWebOC" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_WebOCAppMarkup.xaml" />
           <resource filename="deploy_htmlmarkup.htm" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <ProjectData>
             <PropertyGroup>
                <trustURLParameters>true</trustURLParameters>
             </PropertyGroup>
           </ProjectData>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_WebOCApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

 <!-- End WebOC Scenarios -->

 <!-- Browser-Hosted Content in Frames Scenarios -->

    <AppSecLoaderConfig ID="11">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch and verify a basic Internet-Zone Browser-Hosted app (.xbap) in an HTML Frame, over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="Deploy_ExpressAppInHTMLFrame.htm" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_HtmlFrameXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="12">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch and verify a basic Internet-Zone Browser-Hosted app (.xbap) in a DHTML Frame, over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="Deploy_ExpressAppInDHTMLFrame.htm" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_DHtmlFrameXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="13">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch and verify a basic Internet-Zone Browser-Hosted app (.xbap) in an HTML Frame, over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="Deploy_ExpressAppInIFrame.htm" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_IFrameXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>
 <!-- END Browser-Hosted Content in Frames Scenarios -->

 <!-- History and Favorites Scenarios -->

    <AppSecLoaderConfig ID="14">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch, verify, then relaunch via Favorites a basic Internet-Zone Browser-Hosted app (.xbap) over 1st three common schemes (Local, UNC, HTTP Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapFavorites1.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="15">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch, verify, then relaunch via Favorites a basic Internet-Zone Browser-Hosted app (.xbap) over 2nd set of common schemes (HTTP Internet, HTTPS Inter-/Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapFavorites2.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="16">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch, verify, then relaunch via History a basic Internet-Zone Browser-Hosted app (.xbap) over 1st three common schemes (Local, UNC, HTTP Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="Deploy_Markup1.xaml" SaveToPath="$(OutputPath)" />
           <resource filename="Deploy_Picture1.jpg" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapHistory1.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="17">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch, verify, then relaunch via History a basic Internet-Zone Browser-Hosted app (.xbap) over 2nd set of common schemes (HTTP Internet, HTTPS Inter-/Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="Deploy_Markup1.xaml" SaveToPath="$(OutputPath)" />
           <resource filename="Deploy_Picture1.jpg" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapHistory2.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="18">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch, verify, then relaunch via Favorites a simple .application over 1st three common schemes (Local, UNC, HTTP Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_AppFavorites1.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>   

    <AppSecLoaderConfig ID="19">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch, verify, then relaunch via Favorites a simple .application over 2nd set of common schemes (HTTP Internet, HTTPS Inter-/Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_AppFavorites2.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
 
    </AppSecLoaderConfig>   

    <AppSecLoaderConfig ID="20">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch, verify, then relaunch via IE History a simple .application over 1st three common schemes (Local, UNC, HTTP Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_AppHistory1.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>   

    <AppSecLoaderConfig ID="21">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch, verify, then relaunch via IE History a simple .application over 2nd set of common schemes (HTTP Internet, HTTPS Inter-/Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_AppHistory2.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>   

 <!-- END History and Favorites Scenarios -->

 <!-- Url Parameter Scenarios   -->

    <AppSecLoaderConfig ID="22">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Pass URL parameters to an .xbap over all schemes that use this (HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapUrlParams.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="23">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Pass URL parameters to a .application over all schemes that use this (HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                <trustURLParameters>true</trustURLParameters>
             </PropertyGroup>
           </ProjectData>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_AppUrlParams.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

 <!-- END Url Parameter Scenarios -->

 <!-- Miscellaneous ...  --> 
    <!-- Click "Do Not Run" on Standalone .application on the trust prompt -->
    <AppSecLoaderConfig ID="24">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>2</Priority>        
        <Description> Relaunch standalone .application from the start menu over (Local, UNC, HTTP Intranet)</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_DontRunApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <!-- Relaunch Standalone .application from the start menu shortcut -->
    <AppSecLoaderConfig ID="25">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>2</Priority>        
        <Description> Click "Do not run" button on Trust dialog for .application from the start menu over all schemes </Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_RelaunchApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="26">
        <Owner>MattGal</Owner>
        <SubFeature>Security</SubFeature>
        <Priority>1</Priority>        
        <Description>Load content in WebOC Frame, then validate that this file can still be opened.  Regression Prevention for bug #</Description>

        <BuildInfo>
           <application name="ExpressAppWebOC" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_WebOCXbapMarkup.xaml" />
           <resource filename="deploy_htmlmarkup.htm" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup1.xaml" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_WebOCFileLocking.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="27">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch an .xbap twice, validate that app does not go through downloading state on 2nd run</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapRelaunchOptimization.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>


    <AppSecLoaderConfig ID="28">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Click on unknown SOO content, make sure File Download dialog pops </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="Deploy_un.known" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapUnknownContent.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="29">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Read and write from the isolated store for .xbap over all schemes </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapIsoStorage.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="30">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Try to run app depending on Avalon 7.0, verify appropriate error UI shows </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapReqNotMet.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="31">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>0</Priority>        
        <Description>Run an .xbap 3x and make sure that it renders all 3 times.</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="deploy_contentRendered.png" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapNthRunRendering.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>


    <AppSecLoaderConfig ID="32">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Make sure PresentationHost fires a named event when launched with -event argument</Description>

        <BuildInfo>
           <application name="PresentationHostRegistrationTest" />
           <applicationtype entrypoint="Microsoft.Test.Windows.Client.AppSec.Deployment.PresentationHostEventTest" name=".winexe" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_PresHostEventTest.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

 <!-- End Miscellaneous ...  --> 

 <!-- Browser Interop Scenarios --> 

    <AppSecLoaderConfig ID="33">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Make sure Forward + back buttons, Alt-Right/Left, Backspace and Alt-Backspace work with WinFX Content</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_IEFwdBkAccelKeyTest.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="34">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Make sure Ctrl-P, F11, Ctrl-W all work with avalon content in the browser</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_IECtrlWAndF11.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="35">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Make sure Ctrl-H,I,J work as expected (IE7) when showing browserhosted content</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_LinksExplorerAccelKeys.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="36">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Make sure Avalon control retains focus when switching between browser tabs</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_IE7TabFocusTest.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="37">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Regression test for WOS Bug #1637143 </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_htmlmarkup.htm" SaveToPath="$(OutputPath)" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_IE71637143RegressionTest.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="38" >
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>0</Priority>        
        <Description>Make sure all expected IE menus are present with Avalon content loaded</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup1.xaml" SaveToPath="$(OutputPath)" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_IEMenuMerging.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>


    <AppSecLoaderConfig ID="39">
        <Owner>MattGal</Owner>
        <SubFeature>Regression</SubFeature>
        <Priority>0</Priority>        
        <Description>Load content in WebOC Frame, move focus around the control, failing if exception thrown.  Regression Prevention for bug #1453368</Description>

        <BuildInfo>
           <application name="ExpressAppWebOC" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_WebOCXbapMarkup.xaml" />
           <resource filename="deploy_htmlmarkup.htm" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup1.xaml" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_WebOCFocusRegression.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="40">
        <Owner>MattGal</Owner>
        <SubFeature>Regression</SubFeature>
        <Priority>1</Priority>        
        <Description>Load content in WebOC Frame, navigate away, making sure that webOC object is disposed.  Regression Prevention for bug #1521096</Description>

        <BuildInfo>
           <application name="StandaloneAppWebOC" />
           <applicationtype name="application" />
           <startupuri filename="Deploy_WebOCAppMarkup.xaml" />
           <resource filename="Deploy_XAMLInIFrame.htm" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup1.xaml" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_WebOCDisposedOnNavigation.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>


    <AppSecLoaderConfig ID="41" >
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch an Xbap inside of a WebOC inside Xbap</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup1.xaml" SaveToPath="$(OutputPath)" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
      
        <BuildInfo>
           <application name="ExpressAppWebOC" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_WebOCXbapMarkup.xaml" />
           <resource filename="deploy_htmlmarkup.htm" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)" />
           <resource filename="deploy_markup1.xaml" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_NestedWebOCXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="42" >
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch 3 xbaps in 3 separate browser tabs.  Needs to run elevated on Vista for continuity of behavior.</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <BuildInfo>
           <targetzone name="LocalIntranet" />
           <application name="ExpressAppIntranetZone" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleBrowserHostedNSVApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
                
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapsInIE7Tabs.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="43" >
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch xbaps in Frames in browser tabs.  Needs to run elevated on Vista for continuity of behavior.</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <BuildInfo>
           <targetzone name="LocalIntranet" />
           <application name="ExpressAppIntranetZone" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="Deploy_IntranetExpressAppInIFrame.htm" SaveToPath="$(OutputPath)" />
           <resource filename="Deploy_ExpressAppInIFrame.htm" SaveToPath="$(OutputPath)" />
        </BuildInfo>
                
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapsInFramesInIE7Tabs.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>
 <!-- End Browser Interop Scenarios --> 

 <!-- Trusted/Untrusted Publisher Scenarios --> 

    <AppSecLoaderConfig ID="44">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Verify behavior for .Xbap signed with Trusted Publisher certificate</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <resource filename="AvalonTestTrusted.pfx" />
           <resource filename="AvalonTestTrusted.pfx" SaveToPath="$(OutputPath)"/>
           <application name="TrustedExpressApp" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                <ManifestKeyFile>AvalonTestTrusted.pfx</ManifestKeyFile>
                <ManifestCertificateThumbprint>f625cdcb267029f36d96e4a4629c3b8c8e8156b6</ManifestCertificateThumbprint>
             </PropertyGroup>
             <ItemGroup>
                <None Include="AvalonTestTrusted.pfx" />
             </ItemGroup>
           </ProjectData>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_TrustedXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="45">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Verify behavior for .Xbap signed with UN-Trusted Publisher certificate</Description>

        <BuildInfo>
           <resource filename="AvalonTestUntrusted.pfx" />
           <resource filename="AvalonTestUntrusted.pfx" SaveToPath="$(OutputPath)"/>
           <application name="UntrustedExpressApp" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                <ManifestKeyFile>AvalonTestUntrusted.pfx</ManifestKeyFile>
                <ManifestCertificateThumbprint>3551ca0f3c58c64d6f19fcc01c29259bee494839</ManifestCertificateThumbprint>
             </PropertyGroup>
             <ItemGroup>
                <None Include="AvalonTestUntrusted.pfx" />
             </ItemGroup>
           </ProjectData>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_UntrustedXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="46">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Verify behavior for .Application signed with Trusted Publisher certificate</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <resource filename="AvalonTestTrusted.pfx" />
           <resource filename="AvalonTestTrusted.pfx" SaveToPath="$(OutputPath)"/>
           <application name="TrustedStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />

           <ProjectData>
             <PropertyGroup>
                <ManifestKeyFile>AvalonTestTrusted.pfx</ManifestKeyFile>
                <ManifestCertificateThumbprint>f625cdcb267029f36d96e4a4629c3b8c8e8156b6</ManifestCertificateThumbprint>
             </PropertyGroup>
             <ItemGroup>
                <None Include="AvalonTestTrusted.pfx" />
             </ItemGroup>
           </ProjectData>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_TrustedApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="47">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Verify Behavior for .Xbap signed with UN-Trusted Publisher certificate</Description>

        <BuildInfo>
           <resource filename="AvalonTestUntrusted.pfx" />
           <resource filename="AvalonTestUntrusted.pfx" SaveToPath="$(OutputPath)"/>
           <targetzone name="FullTrust" />
           <application name="UntrustedStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                <ManifestKeyFile>AvalonTestUntrusted.pfx</ManifestKeyFile>
                <ManifestCertificateThumbprint>3551ca0f3c58c64d6f19fcc01c29259bee494839</ManifestCertificateThumbprint>
             </PropertyGroup>
             <ItemGroup>
                <None Include="AvalonTestUntrusted.pfx" />
             </ItemGroup>
           </ProjectData>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_UntrustedApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

 <!-- End Trusted/Untrusted Publisher Scenarios --> 

 <!-- Update /Rollback Scenarios (.application specific) -->
    <AppSecLoaderConfig ID="48">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Update .Application from 1.0 to 1.1 over all common schemes</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                 <ApplicationVersion>1.0.0.*</ApplicationVersion>
                 <PublisherName>Avalon</PublisherName>
                 <UpdateEnabled>true</UpdateEnabled>
                 <UpdateMode>Foreground</UpdateMode>
                 <UpdateInterval>7</UpdateInterval>
                 <UpdateIntervalUnits>Days</UpdateIntervalUnits>
                 <UpdatePeriodically>false</UpdatePeriodically>
                 <UpdateRequired>false</UpdateRequired>
                 <UpdateUrlEnabled>false</UpdateUrlEnabled>
             </PropertyGroup>
           </ProjectData>
        </BuildInfo>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                 <ApplicationVersion>1.1.0.*</ApplicationVersion>
                 <PublisherName>Avalon</PublisherName>
                 <UpdateEnabled>true</UpdateEnabled>
                 <UpdateMode>Foreground</UpdateMode>
                 <UpdateInterval>7</UpdateInterval>
                 <UpdateIntervalUnits>Days</UpdateIntervalUnits>
                 <UpdatePeriodically>false</UpdatePeriodically>
                 <UpdateRequired>false</UpdateRequired>
                 <UpdateUrlEnabled>false</UpdateUrlEnabled>
             </PropertyGroup>
           </ProjectData>
           <OutputToSubDir>Update</OutputToSubDir>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_UpdateApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

<AppSecLoaderConfig ID="49">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Update .Application from 1.0 to 1.1, then roll back to 1.0, over all common schemes</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                 <ApplicationVersion>1.0.0.*</ApplicationVersion>
                 <PublisherName>Avalon</PublisherName>
                 <UpdateEnabled>true</UpdateEnabled>
                 <UpdateMode>Foreground</UpdateMode>
                 <UpdateInterval>7</UpdateInterval>
                 <UpdateIntervalUnits>Days</UpdateIntervalUnits>
                 <UpdatePeriodically>false</UpdatePeriodically>
                 <UpdateRequired>false</UpdateRequired>
                 <UpdateUrlEnabled>false</UpdateUrlEnabled>
             </PropertyGroup>
           </ProjectData>
        </BuildInfo>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                 <ApplicationVersion>1.1.0.*</ApplicationVersion>
                 <PublisherName>Avalon</PublisherName>
                 <UpdateEnabled>true</UpdateEnabled>
                 <UpdateMode>Foreground</UpdateMode>
                 <UpdateInterval>7</UpdateInterval>
                 <UpdateIntervalUnits>Days</UpdateIntervalUnits>
                 <UpdatePeriodically>false</UpdatePeriodically>
                 <UpdateRequired>false</UpdateRequired>
                 <UpdateUrlEnabled>false</UpdateUrlEnabled>
             </PropertyGroup>
           </ProjectData>
           <OutputToSubDir>Update</OutputToSubDir>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_RollBackApp.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

 <!-- End Update /Rollback Scenarios (.application specific) -->

    <AppSecLoaderConfig ID="50">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Build the same app 5x, corrupt in in five ways, make sure the appropriate UI shows each time</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <OutputToSubDir>Copy1</OutputToSubDir>
        </BuildInfo>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <OutputToSubDir>Copy2</OutputToSubDir>
        </BuildInfo>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <OutputToSubDir>Copy3</OutputToSubDir>
        </BuildInfo>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <OutputToSubDir>Copy4</OutputToSubDir>
        </BuildInfo>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <OutputToSubDir>Copy5</OutputToSubDir>
        </BuildInfo>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <OutputToSubDir>Copy6</OutputToSubDir>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapInvalidManifests.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="51">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Make sure PresentationHost.exe exits when IE does no matter what </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <BuildInfo>
           <application name="PresentationHostOrphanTest" />
           <applicationtype entrypoint="Microsoft.Test.Windows.Client.AppSec.Deployment.PresentationHostOrphanMitigation" name=".winexe" />
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_PHostOrphanMitigation.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="52">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Make sure PresentationHost.exe does not respawn on exit</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
        </BuildInfo>

        <BuildInfo>
           <application name="PresentationHostRespawnTest" />
           <applicationtype entrypoint="Microsoft.Test.Windows.Client.AppSec.Deployment.PresentationHostNoRespawnTest" name=".winexe" />
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_PHostRespawnBehavior.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>


    <AppSecLoaderConfig ID="53">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Run .xbap and .application of same name, verify behavior</Description>

        <BuildInfo>
          <application name="SimpleStandAloneApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <OutputToSubDir>Xbap</OutputToSubDir>
        </BuildInfo>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <OutputToSubDir>App</OutputToSubDir>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_ClickOnceSameName.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="54">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Cancel download of .xbap over all schemes using UI Cancel button</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapCancelUIButton.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="55">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Cancel download of .xbap over all schemes using IE Stop button</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapCancelStopBtn.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="56">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Cancel download of .xbap over all schemes using the Escape Key</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapCancelEscKey.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="57">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Cancel download of .xbap, then restart it over all schemes using IE Stop button</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapCancelRestart.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="58">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Cancel download of .xbap by closing browser while app is downloading</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapCancelCloseBrowser.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="59">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Cancel download of .xbap by navigating away while app is downloading</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
          <includegeneratedbitmap/>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_XbapCancelNavigateAway.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="60">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Cancel download of .Application over all normal schemes </Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <includegeneratedbitmap/>
           <includegeneratedbitmap/>
           <includegeneratedbitmap/>
           <includegeneratedbitmap/>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_AppCancel.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

  <!-- Loose Xaml Scenarios -->
  
    <AppSecLoaderConfig ID="61">
        <Owner>MattGal</Owner>
        <SubFeature>Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch loose XAML over all normal schemes </Description>

        <RunInfo>
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Deployment_LaunchLooseXaml.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="62">
        <Owner>MattGal</Owner>
        <SubFeature>Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Navigate to loose XAML over all normal schemes </Description>

        <RunInfo>
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Deployment_NavigateLooseXaml.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="63">
        <Owner>MattGal</Owner>
        <SubFeature>Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch loose XAML in an IFrame over all normal schemes </Description>

        <RunInfo>
           <resource filename="deploy_XamlInIFrame.htm" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup1.xaml"     SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup2.xaml"     SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg"     SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Deployment_XamlinIFrame.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

 <!-- Install local, Update over HTTP -->
    <AppSecLoaderConfig ID="64">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Install .Application over Local, update over HTTP</Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                 <ApplicationVersion>1.0.0.*</ApplicationVersion>
                 <PublisherName>Avalon</PublisherName>
                 <UpdateEnabled>true</UpdateEnabled>
                 <UpdateMode>Foreground</UpdateMode>
                 <UpdateInterval>7</UpdateInterval>
                 <UpdateIntervalUnits>Days</UpdateIntervalUnits>
                 <UpdatePeriodically>false</UpdatePeriodically>
                 <UpdateRequired>false</UpdateRequired>
                 <UpdateUrlEnabled>True</UpdateUrlEnabled>
                 <SupportUrl>www.microsoft.com</SupportUrl>
                 <UpdateUrl>http://wpfapps/testscratch/WebUpdate/</UpdateUrl>
                 <trustURLParameters>true</trustURLParameters>
                 <GenerateManifests>true</GenerateManifests>
             </PropertyGroup>
           </ProjectData>
        </BuildInfo>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleStandAloneApplication" />
           <applicationtype name=".application" />
           <startupuri filename="Deploy_BasicAppMarkup.xaml" />
           <ProjectData>
             <PropertyGroup>
                 <ApplicationVersion>1.1.0.*</ApplicationVersion>
                 <PublisherName>Avalon</PublisherName>
                 <SupportUrl>www.microsoft.com</SupportUrl>
                 <UpdateEnabled>true</UpdateEnabled>
                 <UpdateMode>Foreground</UpdateMode>
                 <UpdateInterval>7</UpdateInterval>
                 <UpdateIntervalUnits>Days</UpdateIntervalUnits>
                 <UpdatePeriodically>false</UpdatePeriodically>
                 <UpdateRequired>false</UpdateRequired>
                 <UpdateUrlEnabled>false</UpdateUrlEnabled>
             </PropertyGroup>
           </ProjectData>
           <OutputToSubDir>Update</OutputToSubDir>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_LocalAppWebUpdate.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

  <!-- PresHost debug mode -->
    <AppSecLoaderConfig ID="65">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Run an app while PresentationHost.exe is running another in -debug mode</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup2.xaml" />
           <OutputToSubDir>different</OutputToSubDir>
        </BuildInfo>

        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_PresHostDebug.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

  <!-- Inter-Xaml navigation scenarios -->
    <AppSecLoaderConfig ID="66">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Navigate between two .Xaml files over all common schemes, local->local, local->unc, etc</Description>

        <RunInfo>
           <resource filename="deploy_markup1.xaml"     SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup2.xaml"     SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg"     SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Deployment_XamlNavigation1.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

  <!-- Xaml->HTML navigation scenarios -->
    <AppSecLoaderConfig ID="67">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Navigate between HTML and .Xaml files over all common schemes, local->local, local->unc, etc</Description>

        <RunInfo>
           <resource filename="deploy_markup1.xaml"     SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup2.xaml"     SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_htmlmarkup.htm"   SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg"     SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Deployment_XamlNavigation2.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>
  <!-- Use Ctrl-N to open a new window with an Xbap -->
    <AppSecLoaderConfig ID="68">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>2</Priority>        
        <Description>Launch .xbap in new window with Ctrl-N</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_XbapCtrlN.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

  <!-- SEE Xbap -->
    <AppSecLoaderConfig ID="69">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description> Refresh testing - Ctrl-R, F5, Ctrl-F5</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <resource filename="deploy_markup1.xaml"     SaveToPath="$(OutputPath)"/>
          <resource filename="deploy_picture1.jpg"     SaveToPath="$(OutputPath)"/>
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_RefreshAccelKeys.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="70">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch .xbap and download "on-demand" assembly over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <AppXaml FileName="Deploy_OnDemandAppDef.xaml"/>
          <!-- startupuri filename="Deploy_OnDemandXbap.xaml" / -->

          <resource filename="Deploy_OnDemandXbap.xaml" />
          <resource filename="Deploy_OnDemandAppDef.xaml" />

          <resource filename="Deploy_SimpleOnDemandAssembly.cs"  />
          <resource filename="Deploy_SimpleOnDemandAssembly.csproj"  />

          <ProjectData>
              <ItemGroup>
                <Page Include="Deploy_OnDemandXbap.xaml" />
              </ItemGroup>

              <ItemGroup>
                <ProjectReference Include="Deploy_SimpleOnDemandAssembly.csproj" />
                <Reference Include="System.Deployment" />
              </ItemGroup>

              <ItemGroup>
                <PublishFile Include="SimpleOnDemandAssembly">
                  <FileType>Assembly</FileType>
                  <InProject>False</InProject>
                  <Group>OnDemandGroup</Group>
                  <PublishState>Auto</PublishState>
                </PublishFile>
              </ItemGroup>
          </ProjectData>

        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_OnDemandXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="71">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Integration</SubFeature>
        <Priority>1</Priority>        
        <Description>Make sure that in IE7 we can tab from browser to inside the docobj</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <resource filename="deploy_markup1.xaml"     SaveToPath="$(OutputPath)"/>
          <resource filename="deploy_picture1.jpg"     SaveToPath="$(OutputPath)"/>
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_IE7BrowserToDocObjFocus.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="72">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Integration</SubFeature>
        <Priority>1</Priority>        
        <Description>IE7 Key filtering tests... makes sure an app gets first chance at "non-sacred" input, F6/Shift-F6 both work, and arrow keys change focus as expected</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_IE7InputFiltering1.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="73">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Make sure that debugsecurityzoneurl works correctly for -debug scenarios </Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
          <resource filename="deploy_markup2.xaml" />
          <resource filename="deploy_picture1.jpg" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_PresHostSOODebug.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="74">
        <Owner>MattGal</Owner>
        <SubFeature>Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Print from loose Xaml, with normal, Flow, and Fixed document paths</Description>

        <RunInfo>
           <resource filename="Deploy_XamlFlowDoc.xaml"  SaveToPath="$(OutputPath)"/>
           <resource filename="Deploy_XamlFixedDoc.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup1.xaml"      SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg"      SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Deployment_BrowserContentPrinting.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="75">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Exercise app with Ctrl-S Command binding (Mostly for Code Coverage :) )</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_XbapCtrlSBinding.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>


    <AppSecLoaderConfig ID="76">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Make sure PresentationHost exits after a timeout period</Description>

        <BuildInfo>
           <application name="PresentationHostTimeoutTest" />
           <applicationtype entrypoint="Microsoft.Test.Windows.Client.AppSec.Deployment.PresentationHostTimeOutTest" name=".winexe" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_PresHostTimeoutTest.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="77">
        <Owner>MattGal</Owner>
        <SubFeature>Deployment</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch and verify a Code-only Browser-Hosted app (.xbap)</Description>

        <BuildInfo>
          <Resource FileName="Deploy_CodeOnlyXbap.cs" IsLoose="True" />
          <Resource FileName="Deploy_CodeOnlyXbap.proj" IsLoose="True" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_CodeOnlyXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>
  
  <!-- SEE Xbap with UTF path -->
    <AppSecLoaderConfig ID="78">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch and verify a basic Internet-Zone Browser-Hosted app (.xbap) over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet) with UTF-8 characters in the path</Description>

        <BuildInfo>
          <application name="Сценарий" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_IE_Internet_XBAP_UTF.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>


  <!-- Test the UserAgent string attached to a webrequest, make sure it's IE specific -->
    <AppSecLoaderConfig ID="79">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Create a webrequest and check that the user agent string attached is "correct"</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_Deployment_IE_WebrequestUAString.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

   <!-- WebOC Regression Scenario : Regression test -->
    <AppSecLoaderConfig ID="80">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Load HTML in WebOC frame inside .xbap.  Click on combo box inside HTML frame, make sure no exception is thrown.</Description>

        <BuildInfo>
           <application name="ExpressAppWebOC" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_WebOCXbapMarkup.xaml" />
           <resource filename="1_Repro.html" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_WebOC_Regression_1.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
 
    </AppSecLoaderConfig>

    <!-- Journal-out-of-sync Regression Scenario: Regression test for DD bug # 120574 -->
    <AppSecLoaderConfig ID="81">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Cancel navigation from with an app (IE7 only).  Make sure browser travellog does not get out of sync with actual navigation</Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_120574_Regression.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <!-- Journal-out-of-sync Regression Scenario: Regression test 1 -->
    <AppSecLoaderConfig ID="82">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Navigate back and forth in an Xbap using the IE7 browser buttons 50x.  Fail if Xbap crashes. </Description>

        <BuildInfo>
          <application name="SimpleBrowserHostedApplication" />
          <applicationtype name="xbap" />
          <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
          <appmonitorconfigfile filename="AMC_1_Regression.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="83">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch and verify two basic Internet-Zone Browser-Hosted apps (.xbap) in an IFrame, over mixed mode schemes (HTTPS Intranet + HTTP Internet) with HTTPS top level</Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="MixedHttpHttpsContent.htm" SaveToPath="$(OutputPath)" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Deployment_HTTPSMixedIframeXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>


</AppSecLoaderConfigs>