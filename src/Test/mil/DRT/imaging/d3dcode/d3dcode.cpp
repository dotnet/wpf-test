#include "d3dcode.h"

struct CUSTOMVERTEX
{
    FLOAT x, y, z; 
    DWORD color;
};

#define D3DFVF_CUSTOMVERTEX (D3DFVF_XYZ | D3DFVF_DIFFUSE)

const static TCHAR szAppName[] = TEXT("shh!");

Renderer::Renderer() : m_pd3dDevice(NULL), m_pd3dVB(NULL), m_pd3dSurface(NULL), m_hwndDummy(NULL)
{

}


HRESULT
Renderer::Create(Renderer **ppRenderer, bool fForceXDDM)
{
    HRESULT hr = S_OK;

    Renderer *pRenderer = new Renderer();

    IFC(pRenderer->Init(fForceXDDM));

    *ppRenderer = pRenderer;

Cleanup:
    return hr;
}

typedef HRESULT (WINAPI *DIRECT3DCREATE9EXFUNCTION)(UINT SDKVersion, IDirect3D9Ex**);

DWORD
GetVertexProcessing(IDirect3D9 *pD3D)
{
    D3DCAPS9 caps;
 
    if (   SUCCEEDED(pD3D->GetDeviceCaps(0, D3DDEVTYPE_HAL, &caps))
        && (caps.DevCaps & D3DDEVCAPS_HWTRANSFORMANDLIGHT) == D3DDEVCAPS_HWTRANSFORMANDLIGHT)
    {
        return D3DCREATE_HARDWARE_VERTEXPROCESSING;
    }
    else
    {
        return D3DCREATE_SOFTWARE_VERTEXPROCESSING;
    }
}

HRESULT
Renderer::InitDevice(bool fForceXDDM)
{
    D3DPRESENT_PARAMETERS d3dpp;
    ZeroMemory(&d3dpp, sizeof(d3dpp));
    d3dpp.Windowed = TRUE;
    d3dpp.BackBufferFormat = D3DFMT_UNKNOWN;
    d3dpp.BackBufferHeight = 1;
    d3dpp.BackBufferWidth = 1;
    d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;

    HMODULE hD3D = LoadLibrary(TEXT("d3d9.dll"));
    DIRECT3DCREATE9EXFUNCTION pfnCreate9Ex = (DIRECT3DCREATE9EXFUNCTION)GetProcAddress(hD3D, "Direct3DCreate9Ex");
    FreeLibrary(hD3D);

    IDirect3DDevice9Ex *pd3dDevice = NULL;
    IDirect3D9 *pD3D = NULL;
    IDirect3D9Ex *pD3DEx = NULL;
    
    HRESULT hr = S_OK;
    if (pfnCreate9Ex)
    {
        hr = (*pfnCreate9Ex)(D3D_SDK_VERSION, &pD3DEx);
    }

    if (pfnCreate9Ex && SUCCEEDED(hr) && !fForceXDDM)
    {
        IFC(pD3DEx->CreateDeviceEx(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, NULL, GetVertexProcessing(pD3DEx) | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE, &d3dpp, NULL, &pd3dDevice));
        IFC(pd3dDevice->QueryInterface(__uuidof(IDirect3DDevice9), reinterpret_cast<void**>(&m_pd3dDevice)));  
    }
    else
    {        
        WNDCLASS wndclass;
        
        wndclass.style = CS_HREDRAW | CS_VREDRAW;
        wndclass.lpfnWndProc = DefWindowProc;
        wndclass.cbClsExtra = 0;
        wndclass.cbWndExtra = 0;
        wndclass.hInstance = NULL;
        wndclass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
        wndclass.hCursor = LoadCursor(NULL, IDC_ARROW);
        wndclass.hbrBackground = (HBRUSH) GetStockObject (WHITE_BRUSH);
        wndclass.lpszMenuName = NULL;
        wndclass.lpszClassName = szAppName;

        if (!RegisterClass(&wndclass))
        {
            IFC(E_FAIL);
        }

        // A non-null hwnd is necessary on XDDM but it doesn't do anything so make one up
        m_hwndDummy = CreateWindow(szAppName,
                            TEXT("whee!"),
                            WS_OVERLAPPEDWINDOW,
                            0,                   // Initial X
                            0,                   // Initial Y
                            0,                   // Width
                            0,                   // Height
                            NULL,
                            NULL,
                            NULL,
                            NULL);

        pD3D = Direct3DCreate9(D3D_SDK_VERSION);
        if (!pD3D) 
        {
            IFC(E_FAIL);
        }

        IFC(pD3D->CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, m_hwndDummy, GetVertexProcessing(pD3D) | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE, &d3dpp, &m_pd3dDevice));
    }

    IFC(m_pd3dDevice->CreateRenderTarget(
        1024,
        1024,
        !fForceXDDM ? D3DFMT_A8R8G8B8 : D3DFMT_X8R8G8B8,
        D3DMULTISAMPLE_NONE,
        0,
        !fForceXDDM ? FALSE : TRUE,
        &m_pd3dSurface,
        NULL
        ));

Cleanup:
    if (pD3DEx)
    {
        // QI added an extra ref in the 9Ex case
        SAFE_RELEASE(pd3dDevice);
    }
    
    SAFE_RELEASE(pD3DEx);
    SAFE_RELEASE(pD3D);

    return hr;
}

Renderer::~Renderer()
{
    SAFE_RELEASE(m_pd3dSurface);
    SAFE_RELEASE(m_pd3dVB);
    SAFE_RELEASE(m_pd3dDevice);

    if (m_hwndDummy)
    {
        DestroyWindow(m_hwndDummy);
        UnregisterClass(szAppName, NULL);
    }
}

HRESULT
Renderer::InitVB()
{
    HRESULT hr = S_OK;

    CUSTOMVERTEX vertices[] =
    {
        { -1.0f, -1.0f, 0.0f, 0xffff0000, }, // x, y, z, color
        {  1.0f, -1.0f, 0.0f, 0xff00ff00, },
        {  0.0f,  1.0f, 0.0f, 0xff00ffff, },
    };

    IFC(m_pd3dDevice->CreateVertexBuffer(sizeof(vertices), 0, D3DFVF_CUSTOMVERTEX, D3DPOOL_DEFAULT, &m_pd3dVB, NULL));

    void *pVertices;
    IFC(m_pd3dVB->Lock(0, sizeof(vertices), &pVertices, 0));
    memcpy(pVertices, vertices, sizeof(vertices));
    m_pd3dVB->Unlock();

Cleanup:
    return hr;
}

HRESULT
Renderer::InitCamera()
{
    HRESULT hr = S_OK;

    // Set up our view matrix. A view matrix can be defined given an eye point,
    // a point to lookat, and a direction for which way is up. Here, we set the
    // eye five units back along the z-axis and up three units, look at the
    // origin, and define "up" to be in the y-direction.
    D3DXVECTOR3 vEyePt(0.0f, 0.0f,-5.0f);
    D3DXVECTOR3 vLookatPt(0.0f, 0.0f, 0.0f);
    D3DXVECTOR3 vUpVec(0.0f, 1.0f, 0.0f);
    D3DXMATRIXA16 matView;
    D3DXMATRIXA16 matProj;
    D3DXMatrixLookAtLH(&matView, &vEyePt, &vLookatPt, &vUpVec);
    IFC(m_pd3dDevice->SetTransform(D3DTS_VIEW, &matView));

    // For the projection matrix, we set up a perspective transform (which
    // transforms geometry from 3D view space to 2D viewport space, with
    // a perspective divide making objects smaller in the distance). To build
    // a perpsective transform, we need the field of view (1/4 pi is common),
    // the aspect ratio, and the near and far clipping planes (which define at
    // what distances geometry should be no longer be rendered).
    D3DXMatrixPerspectiveFovLH(&matProj, D3DX_PI / 4, 1.0f, 1.0f, 100.0f);
    IFC(m_pd3dDevice->SetTransform(D3DTS_PROJECTION, &matProj));

Cleanup:
    return hr;
}

HRESULT 
Renderer::Init(bool fForceXDDM)
{
    HRESULT hr = S_OK;

    IFC(InitDevice(fForceXDDM));

    IFC(InitVB());

    IFC(InitCamera());

    IFC(m_pd3dDevice->SetRenderState(D3DRS_CULLMODE, D3DCULL_NONE));
    IFC(m_pd3dDevice->SetRenderState(D3DRS_LIGHTING, FALSE));
    IFC(m_pd3dDevice->SetStreamSource(0, m_pd3dVB, 0, sizeof(CUSTOMVERTEX)));
    IFC(m_pd3dDevice->SetFVF(D3DFVF_CUSTOMVERTEX));
    IFC(m_pd3dDevice->SetRenderTarget(0, m_pd3dSurface));

Cleanup:
    return hr;
}

HRESULT 
Renderer::Render()
{
    HRESULT hr = S_OK;
    D3DXMATRIXA16 matWorld;

    // might not be created yet
    if (!m_pd3dDevice)
    {
        goto Cleanup;
    }

    IFC(m_pd3dDevice->BeginScene());

    IFC(m_pd3dDevice->Clear(0, NULL, D3DCLEAR_TARGET, D3DCOLOR_XRGB(0, 0, 255), 1.0f, 0));

    // Set up the rotation
    UINT  iTime  = GetTickCount() % 1000;
    FLOAT fAngle = iTime * (2.0f * D3DX_PI) / 1000.0f;
    D3DXMatrixRotationY(&matWorld, fAngle);
    IFC(m_pd3dDevice->SetTransform(D3DTS_WORLD, &matWorld));

    IFC(m_pd3dDevice->DrawPrimitive(D3DPT_TRIANGLELIST, 0, 1));
    
    IFC(m_pd3dDevice->EndScene());

Cleanup:
    return hr;
}
