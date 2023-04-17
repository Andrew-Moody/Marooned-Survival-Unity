

// Some Notes on using custom editors and property drawers

/* I had some missconceptions going in that caused quite a few issues
 * 
 * Unity seems to like to think that every custom class derives from Unity.object (Monobehaviour or ScriptableObject)
 * as a result it is not easy to get a reference to the underlying object from serialisedObject or serializedProperty
 * Value types are okay because serializedProperty has functions to read the values
 * for regular classes and structs I think the only way to get a reference is with FieldInfo (yuck)
 * This can be quite difficult for arrays and nested objects requiring string manipulation of path data
 * 
 * 
 * Another issue is that creating a custom editor (deriving from Editor) only controls the display of an object if that
 * object is the currently selected object. It does nothing when that object is a field of the inspector target
 * 
 * 
 * PropertyDrawers do effect objects even when they are a property field on the inspected object
 * 
 * I found a comment from 2015 that claims PropertyDrawers cant use GUILayout. This may no longer be the case
 * however EditorGUILayout is different from GUILayout but neither of them gave me any errors in the script but I havent tested
 * Ahh so it seems EditorGUILayout is broken on PropertyDrawers (unity docs 2021 claims performance reasons but it seems its more
 * an issue with not being the root object being inspected) so PropertyDrawers are forced to deal with rects
 * 
 * 
 * They also claimed that unity uses a shared instance to draw all PropertyDrawers of the same type
 * (this is the reason you are passed the serialized property to the draw method)
 * This means local variables are shared (I'm inclined to believe this is still the case) at least for all instances in the same inspected target
 * Editors dont have this limitation as they get there own instance (well you are only ever inspecting one object at a time so it makes sense)
 * PropertyDrawers dont even have a reference to the target or serialisedObject like editors do so you cant initialize in OnEnable anyway
 * 
 * 
 * Because PropertyDrawers support nesting (they dont have the whole inspector window to themselves like editors)
 * You have to define a way to get the total height of the property being drawn and you have to deal with the rect that the property occupies
 * 
 * 
 * 
 * Oh boy and then you have collections...
 * 
 * You cant use a property attribute to target collections (arrays, lists, etc. well unity doesnt event display dictionaries at all)
 * if you do tag an array with an property attribute it will apply to the elements not the collection
 * 
 * There are a few options
 * 
 * option one is to use a editor/drawer for the object that owns the array. This does not target all arrays of that type it only targets the
 * specific owning objects you make editors/drawers for. the simplest option but means duplicate code
 *
 * option two is to make a wrapper class for a collection and target the wrapper with an attribute
 * if I am going to have lots of arrays of different types on lots of different owners this may be nice to bother doing
 * 
 * option three is custom attributes. this is very powerfull but also very complex. I dont think the time is worth it short term at this point
 * but it's worth revisiting in the future especially if i end up doing c# full time
 * 
 * 
 * I did a small test to see the performance implications of not being able to cache serialized properties in a propertyDrawer
 * OnGUI really isnt called that often and it can only be called on a few items at a time (the ones being inspected)
 * to make it simple you can have fields for all the properties you need and a function to get them
 * I just would not call it in OnEnable but rather call it in OnGUI so they always contain the current values
 * 
 */
