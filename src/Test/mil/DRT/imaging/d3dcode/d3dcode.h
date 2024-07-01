#ifndef _D3DCODE_H_
#define _D3DCODE_H_

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#include <windows.h>

#include <d3d9.h>
#include <d3dx9.h>

#define IFC(x) { hr = (x); if (FAILED(hr)) goto Cleanup; }
#define SAFE_RELEASE(x) { if ((x)) { (x)->Release(); (x) = NULL; } }

class Renderer
{
public:
    static HRESULT Create(Renderer **ppRenderer, bool fForceXDDM);

    ~Renderer();

    HRESULT Render();

    IDirect3DSurface9 *GetSurfaceNoRef() { return m_pd3dSurface; }

private:
    Renderer();

    HRESULT Init(bool fForceXDDM);
    HRESULT InitDevice(bool fForceXDDM);
    HRESULT InitVB();
    HRESULT InitCamera();
    HRESULT Flush();
    
    IDirect3DDevice9 *m_pd3dDevice;
    IDirect3DVertexBuffer9 *m_pd3dVB;
    IDirect3DSurface9 *m_pd3dSurface;

    HWND m_hwndDummy;
};

#endif
