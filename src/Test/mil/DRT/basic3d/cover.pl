# Finds all of the Media3D classes using the -cover option to the drt
# then looks to see which of them are used in TestCase.cs and prints
# out a report.

$_ = `obj/i386/DrtBasic3D.exe -cover`;
s/System.Windows.Media3D.//g;
@types = split;


open( TESTS, "TestCase.cs" );
@f = <TESTS>;
$f = join( " ", @f);

@referenced = grep {(index $f," $_")!=-1} @types;
@notreferenced = grep {(index $f," $_")==-1} @types;

print "---- The following Media3D classes ARE used in TestCase.cs ----\n";
print join( "\n", @referenced );
print "\n\n---- The following Media3D classes ARE NOT used in TestCase.cs ----\n";
print join( "\n", @notreferenced );


