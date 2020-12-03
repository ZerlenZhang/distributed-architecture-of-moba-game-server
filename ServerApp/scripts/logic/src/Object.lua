local Object={};

--metamethods
Object.__index=Object;

--class properties
Object.name="[Object.Class.Property.name]name";

--instance methods
function Object:Print()
    print("[Object.Instance.Method.Print] "..self.name);
end

--constructor
function Object:New(name)
    local instance = {};

    --necessary settings
    setmetatable(instance, self);
    instance.__index=instance;

    --instance properies
    instance.name= name or "[Object.Instance.Property.name]name";

    return instance;
end


return Object;