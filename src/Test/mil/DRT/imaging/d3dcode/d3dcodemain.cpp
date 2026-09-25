#include "d3dcode.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

Renderer *pRenderer = NULL;

extern "C" HRESULT WINAPI Init(IDirect3DSurface9 **ppSurface, BOOL fForceXDDM)
{
    HRESULT hr = S_OK;

    IFC(Renderer::Create(&pRenderer, !!fForceXDDM));

    *ppSurface = pRenderer->GetSurfaceNoRef();

Cleanup:
    return hr;
}

extern "C" HRESULT WINAPI Render()
{
    HRESULT hr = S_OK;

    if (!pRenderer)
    {
        IFC(E_FAIL);
    }

    IFC(pRenderer->Render());

Cleanup:
    return hr;
}

extern "C" void WINAPI Destroy()
{
    delete pRenderer;
    pRenderer = NULL;
}

