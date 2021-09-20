#ifndef _RESOURCE_ALLOCATOR_HPP_
#define _RESOURCE_ALLOCATOR_HPP_

#include <unordered_map>
#include <memory>
#include <string>

template <typename T>
class ResourceAllocator
{
public:

    int Add(std::string filePath)
    {
        auto it = resources.find(filePath);
        if (it != resources.end())
        {
            return it->second.first;
        }

        std::shared_ptr<T> resource = std::make_shared<T>();
        //FileIO* fileIO = fileIO->GetInstance();
        
        //fileIO->Read(filePath, resource);
        //resource = fileIO->Read(filePath, resource);
        //if (!fileIO->Read(filePath, resource))
        //{
        //    return -1;
        //}

        if (!resource->LoadFromFile(filePath))
        {
            return -1;
        }

        resources.insert(std::make_pair(filePath, std::make_pair(currentId, resource)));

        return currentId++;
    }

    void Remove(int id)
    {
        for (auto it = resources.begin(); it != resources.end(); ++it)
        {
            if (it->second.first == id)
            {
                resources.erase(it->first);
            }
        }
    }

    std::shared_ptr<T> Get(int id)
    {
        for (auto it = resources.begin(); it != resources.end(); ++it)
        {
            if (it->second.first == id)
            {
                return it->second.second;
            }
        }

        return nullptr;
    }

    bool Has(int id)
    {
        return (Get(id) != nullptr);
    }

    size_t Size()
    {
        return resources.size();
    }

private:
    int currentId;
    std::unordered_map<std::string, std::pair<int, std::shared_ptr<T>>> resources;

};
#endif // _RESOURCE_ALLOCATOR_HPP_
