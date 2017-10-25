#pragma once
#include <vector>
struct DHVertex;
extern "C" {

    void* dh_create();
    void dh_delete(void* ug);

    bool dh_load(void* ug, const char* path);
    void dh_unload(const void* ug);
    
    void dh_get_triangles(const void* ug, void* &start, int* length);
    void dh_get_vectices(const void* ug, void* &start, int* length);
    void dh_get_indices(const void* ug, void* &start, int* length);
    bool dh_findpath(const void* ug, void* start, void* end, void* vertices, int* length);
	bool dh_findpath_cpp(const void* ug, void* start, void* end, std::vector<DHVertex>* path);
    void* dh_createpath();
    void dh_deletepath(void*);
    void* dh_getpathdata(void* path);
    bool dh_getheight(void* ug, float x, float z, float& y);


	bool dh_is_point_near_navmesh(const void* ug, float x, float z); // ��ѯ����xzƽ����Ƿ���navmesh�ϣ��Ǿ�ȷ��
	bool dh_find_straight_path(const void* ug, void* start, void* end, void* vertices, int* length); // Ѱ·���õ��յ㣩

	bool dh_find_straight_path_cpp(void* ug, const DHVertex& start, const DHVertex& end, std::vector<DHVertex>& path); // Ѱ·���õ��յ㣩


}