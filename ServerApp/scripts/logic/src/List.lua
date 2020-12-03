local Object=require("logic/src/Object");
local List=Object:New("[Class] List");

function List:New(name)
    local instance=Object.New(self, "[List] "..name)
    instance.__content={};
    return instance;
end

function List:Count()
    return #self.__content;
end

function List:Add(item)
    self.__content[#self.__content+1]=item;
end

function List:Remove(item)
    local newContent={};
    for i, v in ipairs(self.__content) do
        if v~=item then
            newContent[#newContent+1]=v;
        end
    end
    for i, v in ipairs(self.__content) do
        if i<=#newContent then
            self.__content[i]=v;
        else
            self.__content[i]=nil;
        end
    end
end

function List:Clear()
    self.__content={};
end

function List:Contains(item)
    return self:Any(function(value)return item==value;end);
end

function List:Foreach(action_index_value)
    for i,v in ipairs(self.__content) do
        action_index_value(i,v);
    end
end

function List:FindFirst(func_bool_item)
    for i,v in ipairs(self.__content) do
        if func_bool_item(v) then
            return v;
        end
    end
    return nil;
end

function List:Where(func_bool_item)
    local ans=List:New("Where-"..self.name);
    self:Foreach(function(index,item)
        if func_bool_item(item) then
            ans:Add(item);
        end
    end);
    return ans;
end

function List:All(func_bool_item)
    local ans=true;
    self:Foreach(function(index,item)
        if not func_bool_item(item) then
            ans=false;
        end
    end);
    return ans;
end

function List:Any(func_bool_item)
    local ans=false;
    self:Foreach(function(index,item)
        if func_bool_item(item) then
            ans=true;
        end
    end);
    return ans;
end

function List:Data()
    return self.__content;
end;

return List;