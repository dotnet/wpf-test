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
        <Description>Launch Internet zone .Xbap and verify over all schemes, where FireFox is the default browser (FireFox Specific)  </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxLaunchInternetXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>


    <!-- Intranet zone Xbap -->
    <AppSecLoaderConfig ID="2">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch and verify a basic Intranet-Zone Browser-Hosted app (.xbap) over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet) (FireFox Specific) </Description>

        <BuildInfo>
           <targetzone name="LocalIntranet" />
           <application name="ExpressAppIntranetZone" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxLaunchIntranetXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <!-- Full trust Xbap -->
    <AppSecLoaderConfig ID="3">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch and verify a basic full-trust Browser-Hosted app (.xbap) over all common schemes (Local, UNC, HTTP/HTTPS Inter/Intranet)  (FireFox Specific) </Description>

        <BuildInfo>
           <targetzone name="FullTrust" />
           <application name="SimpleBrowserHostedNSVApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxLaunchFullTrustxbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="4">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch loose XAML over all normal schemes (FireFox Specific) </Description>

        <RunInfo>
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxLaunchLooseXaml.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="5">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch .Xbap in IFrame in HTML over all normal schemes (FireFox Specific) </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
           <resource filename="Deploy_ExpressAppInIFrame.htm" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxXbapIFrame.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="6">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch .Xbap in HTML Frames in HTML over all normal schemes (FireFox Specific) </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>

        <RunInfo>
           <resource filename="Deploy_ExpressAppInHTMLFrame.htm" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxXbapHTMLFrame.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="7">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch loose .xaml files in IFrames in HTML over all normal schemes (FireFox Specific) </Description>

        <RunInfo>
           <resource filename="Deploy_XAMLInIFrame.htm" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup1.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxLooseXamlIFrame.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="8">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch loose .xaml files in IFrames in HTML over all normal schemes (FireFox Specific) </Description>

        <RunInfo>
           <resource filename="Deploy_XAMLInHTMLFrame.htm" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup1.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxLooseXamlHTMLFrame.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>


    <!-- Navigate to Internet Xbap -->
    <AppSecLoaderConfig ID="9">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Navigate (put URL into FF address bar) Internet zone .Xbap and verify over all schemes, where FireFox is the default browser (FireFox Specific)  </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxNavigateInternetXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <!-- Navigate to Loose .Xaml content -->
    <AppSecLoaderConfig ID="10">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Navigate (put URL into FF address bar) loose XAML over all normal schemes (FireFox Specific) </Description>

        <RunInfo>
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxNavigateLooseXaml.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

  <!-- SEE Xbap with UTF path -->
    <AppSecLoaderConfig ID="11">
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
          <appmonitorconfigfile filename="AMC_FF_Internet_XBAP_UTF.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>

    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="12">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>0</Priority>        
        <Description>Launch Internet zone .Xbap and verify over all schemes, where FireFox is the default browser (FireFox Specific)  </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Orcas_FFWebrequestUAString.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <AppSecLoaderConfig ID="13">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch Internet zone .Xbap, then navigate away to path containing non-ASCII characters </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <resource filename="deploy_markup2.xaml" SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxNavAwayToNonASCII.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <!-- Xbap Resizing browser window -->
    <AppSecLoaderConfig ID="14">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch Internet zone .Xbap, ensure that application can resize its window, but only up til the size of its containing display </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxResizeBrowserWindow.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <!-- Xbap navigating away to HTML -->
    <AppSecLoaderConfig ID="15">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch Internet zone .Xbap, Navigate away to HTML page </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <resource filename="deploy_htmlmarkup.htm" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxNavigateAwayToHTML.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <!-- Xbap Input bubbling to browser in FireFox -->
    <AppSecLoaderConfig ID="16">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch Internet zone .Xbap in FireFox, make sure Ctrl-P brings up print dialog and other input is handled appropriately. </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
        </BuildInfo>
        
        <RunInfo>
           <resource filename="deploy_htmlmarkup.htm" SaveToPath="$(OutputPath)"/>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxRBWInputBubbling.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>


    <AppSecLoaderConfig ID="17">
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
           <appmonitorconfigfile filename="AMC_FireFox_HTTPSMixedIframeXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>

    <!-- Refresh Xbap -->
    <AppSecLoaderConfig ID="18">
        <Owner>MattGal</Owner>
        <SubFeature>Browser Hosting</SubFeature>
        <Priority>1</Priority>        
        <Description>Launch Internet zone .Xbap and press refresh in FireFox  </Description>

        <BuildInfo>
           <application name="SimpleBrowserHostedApplication" />
           <applicationtype name="xbap" />
           <startupuri filename="Deploy_BasicXbapMarkup.xaml" />
           <resource filename="deploy_markup1.xaml"     SaveToPath="$(OutputPath)"/>
           <resource filename="deploy_picture1.jpg"     SaveToPath="$(OutputPath)"/>
        </BuildInfo>
        
        <RunInfo>
           <appmonitorconfigfile filename="AMC_Orcas_FireFoxRefreshXbap.xml" SaveToPath="$(OutputPath)" />
        </RunInfo>
    </AppSecLoaderConfig>
  
  
</AppSecLoaderConfigs>