local function range(m,n)
    return math.random(m,n);
end

local function get_ulevel_uexp(curlevel,curExp,exp)
    if curExp+exp>100 then
        return curlevel+1,curExp+exp-100;
    end
    return curlevel,curExp+exp;
end

local function get_urank_urankExp(curRank,curRankExp,exp)
    if curRankExp+exp>125 then
        return curRank+1,curRankExp+exp-125;
    end
    return curRank,curRankExp+exp;
end

return {
    Range=range,
    GetUlevelAndUexp=get_ulevel_uexp,
    GetUrankAndUrankExp=get_urank_urankExp,
};