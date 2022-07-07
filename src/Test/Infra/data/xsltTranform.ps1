param(
[Parameter(Mandatory=$true)]
[string]$xmlDir,
[Parameter(Mandatory=$true)]
[string]$htmlDir
)


function Load-Xml
{
param([string]$filename)

$content = Get-Content $filename

$stream = new-object System.IO.MemoryStream

$writer = new-object System.IO.StreamWriter($stream)
$writer.Write("$content")
$writer.Flush()
$stream.position = 0

$xml = new-object System.Xml.XmlTextReader($stream)

return $xml
}

function Load-Xslt
{
param([string]$filename)

$content = Get-Content $filename
$stream = new-object System.IO.MemoryStream
$writer = new-object System.IO.StreamWriter($stream)
$writer.Write("$content")
$writer.Flush()
$stream.position = 0

$reader = [System.Xml.XmlReader]::create($stream)
$xslt = New-Object System.Xml.Xsl.XslCompiledTransform
$xslt.Load($reader)

return $xslt
}

function PlainHTML
{
param([string]$xmlFileName, [string]$outFileName)
write-output "Processing $xmlFileName for output $outFileName"

$srcFile = [System.IO.StreamReader] $xmlFileName

$data = "<plaintext>" + $srcFile.ReadToEnd() + "</plaintext>"
$streamWriter = [System.IO.StreamWriter] "$outFileName"
$streamWriter.Write($data)
$streamWriter.Flush()
$streamWriter.Close()
}

function TranformXml
{
param([string]$xmlFileName, [string]$xslFileName, [string]$outFileName)

write-output "Processing $xmlFileName using $xslFileName for output $outFileName"

$output = New-Object System.IO.MemoryStream
$reader = new-object System.IO.StreamReader($output)

$arglist = new-object System.Xml.Xsl.XsltArgumentList
$xml = Load-Xml($xmlFileName)
$xslt = Load-Xslt($xslFileName)
$xslt.Transform($xml, $arglist, $output)

$output.position = 0
$transformed = [string]$reader.ReadToEnd()
$reader.Close()

$transformed = $transformed.Replace(".xml", ".html")

$streamWriter = [System.IO.StreamWriter] "$outFileName"
$streamWriter.Write($transformed)
$streamWriter.Flush()
$streamWriter.Close()
}

function TransformDir{
param([string]$xDir, [string]$hDir) 

write-output "Processing Directory $xDir using variationXsl=$variationXsl output=$hDir"

if (![System.IO.Directory]::Exists($hDir)){
    [System.IO.Directory]::CreateDirectory($hDir)
}

$xmlFiles = Get-ChildItem $xDir -Filter *.xml
for ($i=0; $i -lt $xmlFiles.Count; $i++) {
    $xmlFile = $xmlFiles[$i].FullName
    $xslFile = $xmlFiles[$i].FullName.Replace(".xml", ".xsl")

    $htmlFile = [System.IO.Path]::Combine($hDir, $xmlFiles[$i].Name.Replace(".xml", ".html"))
    if (![System.IO.File]::Exists($xslFile)){
        if ($xmlFile.ToLower().Contains("variation")){
            $xslFile = [System.IO.Path]::Combine($PSScriptRoot, "Infra\Reporting\StaticHtmlVariationReport.xsl");
        }
    }
    if ([System.IO.File]::Exists($xslFile)){
        TranformXml $xmlFile  $xslFile $htmlFile
    }
    else{
        PlainHTML $xmlFile $htmlFile
    }

}

$childDirs = Get-ChildItem $xDir -Attributes D
for ($i=0; $i -lt $childDirs.Count; $i++) {
    $childHtmlDir = [System.IO.Path]::Combine($hDir, $childDirs[$i].Name)
    TransformDir $childDirs[$i].FullName  $childHtmlDir
}
}

TransformDir $xmlDir $htmlDir





