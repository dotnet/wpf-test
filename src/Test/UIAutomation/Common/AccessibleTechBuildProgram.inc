#############################################################
## Common build commands across projects
#############################################################

TARGETTYPE=NOTARGET
SOURCES=
CLR_TARGETTYPE=WINEXE
URT_VER=2.0
ASSEMBLY_IDENTITY_PUBLIC_KEY_NAME=
TEST_CODE=1

#PASS2_BINPLACE=$(BINPLACE_EXE)
BINPLACE_DESTINATIONS=FeatureTests\UIAutomation

_NT_TARGET_VERSION=$(_NT_TARGET_VERSION_WINXP)
USE_NATIVE_EH=1