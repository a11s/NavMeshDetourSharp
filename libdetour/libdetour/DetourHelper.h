#pragma once

#include <string>
#include <vector>
#include "DetourCommon.h"
#include "DetourNavMesh.h"
#include "DetourNavMeshBuilder.h"
#include "DetourNavMeshQuery.h"


enum SamplePolyFlags
{
	SAMPLE_POLYFLAGS_WALK = 0x01,		// Ability to walk (ground, grass, road)
	SAMPLE_POLYFLAGS_SWIM = 0x02,		// Ability to swim (water).
	SAMPLE_POLYFLAGS_DOOR = 0x04,		// Ability to move through doors.
	SAMPLE_POLYFLAGS_JUMP = 0x08,		// Ability to jump.
	SAMPLE_POLYFLAGS_DISABLED = 0x10,		// Disabled polygon
	SAMPLE_POLYFLAGS_ALL = 0xffff	// All abilities.
};

enum SamplePolyAreas
{
	SAMPLE_POLYAREA_GROUND,
	SAMPLE_POLYAREA_WATER,
	SAMPLE_POLYAREA_ROAD,
	SAMPLE_POLYAREA_DOOR,
	SAMPLE_POLYAREA_GRASS,
	SAMPLE_POLYAREA_JUMP,
};

// ����ṹ
struct DHVertex
{
	float x; // x����
	float y; // y����
	float z; // z����
};
// �����ͽṹ
struct DHTriangle
{
	DHVertex vertices[3]; // ����
	int indices[3]; // ����
};

class DetourHelper
{
public:
	// 
	DetourHelper();
	//
	~DetourHelper();
	// ���ص�ͼnavmesh����
	bool Load(const std::string& filepath); 
	// ж��navmesh����
	void UnLoad(); 
	// ��ѯ����xzƽ����Ƿ���navmesh�ϣ��Ǿ�ȷ��
	bool IsPointNearNavMesh(float x, float z); 
	// Ѱ·���õ�����·���㣩
	bool FindPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path); 
	// Ѱ·���õ�����·���㣩
	bool FindPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path, dtPolyRef& startPolyRef, dtPolyRef& endPolyRef); 
	// Ѱ·���õ��յ㣩
	bool FindStraightPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path); 
	// Ѱ·���õ��յ㣩
	bool FindStraightPath(const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path, dtPolyRef& startPolyRef, dtPolyRef& endPolyRef); 
	// ��ȡ��xzƽ����һ����navmesh�ϵĸ߶ȣ���ȷ��������õ㲻��navmesh�ϣ���ô����false
	bool GetHeight(float x, float z, float& y); 
	// ��ð�Χ����С�����������
	void GetBound(DHVertex& min, DHVertex& max); 
	// ��ȡ���������б�
	const std::vector<DHTriangle>& GetTriangles() { return m_triangles; } 
	// ��ȡ�������б�
	const std::vector<DHVertex>& GetVertices() { return m_vertices; } 
	// ��ȡ�������б�
	const std::vector<int>& GetIndices() { return m_indices; } 

private:
	std::vector<DHVertex> m_vertices;
	std::vector<int> m_indices;
	std::vector<DHTriangle> m_triangles;
	float m_bmin[3];
	float m_bmax[3];
	dtNavMesh* m_pNavMesh;
	dtNavMeshQuery* m_pNavQuery;
	dtQueryFilter m_filter;
	static const int MAX_POLYS = 256;
	bool m_isLoaded;
	std::string m_filePath;
};

/// Returns the minimum of two values.
///  @param[in]		a	Value A
///  @param[in]		b	Value B
///  @return The minimum of the two values.
template<class T> inline T rcMin(T a, T b) { return a < b ? a : b; }

/// Returns the maximum of two values.
///  @param[in]		a	Value A
///  @param[in]		b	Value B
///  @return The maximum of the two values.
template<class T> inline T rcMax(T a, T b) { return a > b ? a : b; }