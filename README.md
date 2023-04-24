# Ferestar
Frestar is a CFG language grammar which developed with .net7 and that very useful for execute task and process

## The CFG (Context Free Grammer)

A context-free grammar G is defined by the 4-tuple 
G = ( V , Σ , R , S )

1- V is a finite set; each element v ∈ V is called a nonterminal character or a variable. Each variable represents a different type of phrase or clause in the sentence. Variables are also sometimes called syntactic categories. Each variable defines a sub-language of the language defined by G.

2- Σ is a finite set of terminals, disjoint from V, which make up the actual content of the sentence. The set of terminals is the alphabet of the language defined by the grammar G.

3- P is a finite relation in V×(V∪Σ) , where the asterisk represents the Kleene star operation. The members of P are called the (rewrite) rules or productions of the grammar. (also commonly symbolized by a R)

4- S is the start variable (or start symbol), used to represent the whole sentence (or program). It must be an element of V.



## Frestar Grammer
### G: Σ = { d , a , b , c , r , w , o }

c => function name

r => open region eg: { or (

w => close region eg: } or )

d => 'Value'  eg: string or number or date ... 'Bahrami'

a => variable or parameter name

b => Assign symbol  eg: = or :

o => seperator between parameter eg: , or ...

<img src="https://github.com/alirezakoo/FerestarGrammer/blob/master/grammer_image_small.png"  width="400">

##
