////////////////////////////////////////////////////////////////////////////////
// Filename: modelclass.cpp
////////////////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "trackclass.h"
#include <algorithm>


TrackClass::TrackClass()
{
	m_color = 0;
	m_vertexBuffer = 0;
	m_indexBuffer = 0;
	m_maxVertexCount = 128;
	m_maxIndexCount = 2 * (m_maxVertexCount - 1);
	m_points = vector<XMFLOAT3>();
	m_points.reserve(m_maxVertexCount);
}

TrackClass::TrackClass(size_t maxVertices,float red,float green, float blue, float alpha)
{
	m_color = (red < 1.0f && green < 1.0f && blue < 1.0f && alpha < 1.0f 
		&& red >= 0.0f && green >= 0.0f && blue >= 0.0f && alpha >= 0.0f)
		? new XMFLOAT4(red, green, blue, alpha) : NULL;
	m_vertexBuffer = 0;
	m_indexBuffer = 0;
	m_maxVertexCount = maxVertices;
	m_maxIndexCount = 2 * (m_maxVertexCount - 1);
	m_points = vector<XMFLOAT3>();
	m_points.reserve(m_maxVertexCount);
}


TrackClass::TrackClass(const TrackClass& other)
{
}


TrackClass::~TrackClass()
{
}


bool TrackClass::Initialize(ID3D11Device* device)
{
	bool result;


	// Initialize the vertex and index buffers.
	result = InitializeBuffers(device);
	if(!result)
	{
		return false;
	}

	return true;
}


void TrackClass::Shutdown()
{
	// Shutdown the vertex and index buffers.
	ShutdownBuffers();

	return;
}


void TrackClass::Render(ID3D11DeviceContext* deviceContext)
{
	// Put the vertex and index buffers on the graphics pipeline to prepare them for drawing.
	RenderBuffers(deviceContext);

	return;
}

void TrackClass::AddPoint(float x, float y, float z)
{
	if (m_points.size() >= m_maxVertexCount) {
		m_points.erase(m_points.begin());
	}
	m_points.push_back({ x,y,z });
	m_hasChanged = true;
}

bool TrackClass::DeletePoint(float x, float y, float z)
{
	for (auto it = m_points.begin(); it != m_points.end(); it++) {
		if (it->x == x && it->y == y && it->z == z) {
			m_points.erase(it);
			return true;
		}
	}
	return false;
}

void TrackClass::Clear()
{
	m_points.clear();
	m_hasChanged = true;
}


int TrackClass::GetIndexCount()
{
	return m_indexCount;
}


bool TrackClass::InitializeBuffers(ID3D11Device* device)
{
	VertexType* vertices;
	unsigned long* indices;
	D3D11_BUFFER_DESC vertexBufferDesc, indexBufferDesc;
    D3D11_SUBRESOURCE_DATA vertexData, indexData;
	HRESULT result;


	// Set the number of vertices in the vertex array.
	m_vertexCount = m_points.size();

	// Set the number of indices in the index array.
	m_indexCount = 2*(m_points.size()-1);

	// Create the vertex array.
	vertices = new VertexType[m_maxVertexCount]();
	unsigned int indx = 0;
	for_each(m_points.begin(), m_points.end(), [&indx, &vertices,this](XMFLOAT3 point) {
		vertices[indx++] = { point,m_color ? *m_color : TrackClass::GenerateRandomColor() };
	});
	if(!vertices)
	{
		return false;
	}

	// Create the index array.
	indices = new unsigned long[m_maxIndexCount]();
	if(!indices)
	{
		return false;
	}
	for (int i = 0; (i + 1) < m_vertexCount; i++) {
		indices[2*i] = i;
		indices[2 * i + 1] = i + 1;
	}

	// Set up the description of the static vertex buffer.
    vertexBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    vertexBufferDesc.ByteWidth = sizeof(VertexType) * m_maxVertexCount;
    vertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    vertexBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    vertexBufferDesc.MiscFlags = 0;
	vertexBufferDesc.StructureByteStride = 0;

	// Give the subresource structure a pointer to the vertex data.
    vertexData.pSysMem = vertices;
	vertexData.SysMemPitch = 0;
	vertexData.SysMemSlicePitch = 0;

	// Now create the vertex buffer.
    result = device->CreateBuffer(&vertexBufferDesc, &vertexData, &m_vertexBuffer);
	if(FAILED(result))
	{
		return false;
	}

	// Set up the description of the static index buffer.
    indexBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    indexBufferDesc.ByteWidth = sizeof(unsigned long) * m_maxIndexCount;
    indexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    indexBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    indexBufferDesc.MiscFlags = 0;
	indexBufferDesc.StructureByteStride = 0;

	// Give the subresource structure a pointer to the index data.
    indexData.pSysMem = indices;
	indexData.SysMemPitch = 0;
	indexData.SysMemSlicePitch = 0;

	// Create the index buffer.
	result = device->CreateBuffer(&indexBufferDesc, &indexData, &m_indexBuffer);
	if(FAILED(result))
	{
		return false;
	}

	// Release the arrays now that the vertex and index buffers have been created and loaded.
	delete [] vertices;
	vertices = 0;

	delete [] indices;
	indices = 0;

	return true;
}


void TrackClass::ShutdownBuffers()
{
	// Release the index buffer.
	if(m_indexBuffer)
	{
		m_indexBuffer->Release();
		m_indexBuffer = 0;
	}

	// Release the vertex buffer.
	if(m_vertexBuffer)
	{
		m_vertexBuffer->Release();
		m_vertexBuffer = 0;
	}

	return;
}


void TrackClass::RenderBuffers(ID3D11DeviceContext* deviceContext)
{
	if (m_points.empty()) return;
	unsigned int stride;
	unsigned int offset;

	if (m_hasChanged) {
		RefreshBuffers(deviceContext);
		m_hasChanged = false;
	}
	// Set vertex buffer stride and offset.
	stride = sizeof(VertexType); 
	offset = 0;
    
	// Set the vertex buffer to active in the input assembler so it can be rendered.
	deviceContext->IASetVertexBuffers(0, 1, &m_vertexBuffer, &stride, &offset);

    // Set the index buffer to active in the input assembler so it can be rendered.
	deviceContext->IASetIndexBuffer(m_indexBuffer, DXGI_FORMAT_R32_UINT, 0);

    // Set the type of primitive that should be rendered from this vertex buffer, in this case triangles.
	deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_LINELIST);

	return;
}
XMFLOAT4 TrackClass::GenerateRandomColor() {
	return XMFLOAT4(rand() % 100 / 100.0f, rand() % 100 / 100.0f, rand() % 100 / 100.0f, rand() % 100 / 100.0f);
}

void TrackClass::RefreshBuffers(ID3D11DeviceContext* deviceContext)
{
	m_vertexCount = m_points.size();

	// Set the number of indices in the index array.
	m_indexCount = 2 * (m_points.size() - 1);

	// Create the vertex array.
	VertexType* vertices = new VertexType[m_maxVertexCount]();
	unsigned int indx = 0;
	for_each(m_points.begin(), m_points.end(), [&indx, &vertices,this](XMFLOAT3 point) {
		vertices[indx++] = { point,m_color ? *m_color : TrackClass::GenerateRandomColor() };
	});
	// Create the index array.
	unsigned long* indices = new unsigned long[m_maxIndexCount];
	for (int i = 0; i < m_vertexCount - 1; i++) {
		indices[2 * i] = i;
		indices[2 * i + 1] = i + 1;
	}
	D3D11_MAPPED_SUBRESOURCE mappedResource;
	deviceContext->Map(m_vertexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
	//mappedResource.pData = (void*) new VertexType[m_vertexCount];
	memcpy(mappedResource.pData, vertices, sizeof(VertexType)*m_vertexCount);
	deviceContext->Unmap(m_vertexBuffer, 0);
	deviceContext->Map(m_indexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
	memcpy(mappedResource.pData, indices, sizeof(unsigned long)*m_indexCount);
	deviceContext->Unmap(m_indexBuffer, 0);
	delete[] vertices;
	delete[] indices;
}
