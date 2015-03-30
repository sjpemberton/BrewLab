namespace Models

open Units

type HopType =
|Pellet
|Leaf
|Extract

type HopAddition =
|Bittering
|Flavour
|Aroma
|DryHop

type hop = {Name:string; Weight:float<g>; Alpha:float<percentage>; Time:float; Type:HopType; }
type adjunct = {Name:string; Weight:float<g>; Description:string }
type grain = {Name:string; Weight:float<g>; Potential:float<pgpkg>; Colour:float<EBC>}
type yeast = {Name:string; Attenuation:float<percentage>; TempRange: float<degC>*float<degC> }
type water = {Name:string;} //chemical profile

type Ingredient =
| Hop of hop
| Adjunct of adjunct
| Grain of grain