   M S B A M L   `   `   `  ����?  ;CoRo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:CoRoCoRo  5   �        CoRo.WindowGamepadSettings   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6F   $localclr-namespace:CoRo     6�   -   5          � Title$  Gamepad Settings��5   	   $	��620��6"   $	��360��6/    � 
ResizeMode$ NoResize=�6;    � WindowStyle$
 None=�6Q    � WindowStartupLocation$ CenterScreen=�6d   .��5      � .+�5   
   �� $��10q�6   .+�5      �� $��20(�6   $
��Bold"�6"   .��64   	Gamepad6=   � 5	      $
��0,10q�6    � ColumnDefinitions 5
      �� 5      $	��100 �6'   �� 5      $��* �6'   .+�5      �� $��Devices:��6   $��0��6+   $
��Left=�6;   $��Center=�6V   �� 5      -   5         ��Name$ GamepadList��5      $��1��60   $��Stretch=�6@   $��Center=�6^   6y   5      �� 5      $��	Settings:��6   $��Stretch=�6*   $	��Top=�6H   $
��Auto��6`   .��5      �� .+�5      � $��10q�6    5      �� 5      $��* �6/   �� 5      $
��Auto �6/   .+�5      �� -   5         ��Name$ IniListPath��5   "   $��0��67   $��Stretch=�6G   $��50��6e    �� TextWrapping$
 Wrap=�6q   $��Center=�6�   �� 5       �� Orientation$ 
Horizontal=�6%   $��1��6>   $��Center=�6N   $��Right=�6i   $��10,0,0,0q�6�   .+�5      �� -   5        	 ��Name$	 buttonSaveIniList��5   %   $��30��6@   $��30��6L   
������ $��0q�6p   .��5   "   �� $��./Resources/saveicon.png��6(   $	��5,5q�6J   5      �� 5      -   5        $	 buttonLoadIniList��5   %   $��30��6@   $��30��6L   
������ $��0q�6p   .��5   "   �� $��./Resources/openicon.png��6(   $	��5,5q�6J   5      5       5!      � 5"      $��10q�6    5#      �� 5$      $��* �6/   �� 5%      $��* �6/   .+�5'      �� $��0��6%   $
��Left=�65   $	��Top=�6P   .+�5(      ,   &CoRo.UserControlButtonModifierSelector  -   5        
  Name$
 controlX��5(   F   ,   &CoRo.UserControlButtonModifierSelector   Click$ controlX_Click��6b      Label$ X��6X   6b     5)      -   5        $
 controlY��5)   F   $ controlY_Click��6b   $ Y��6X   6b     5*      -   5        $
 controlZ��5*   F   $ controlZ_Click��6b   $ Z��6X   6b     5+      -	   5        $
 controlA��5+   F   $ controlA_Click��6b   $ A��6X   6b     5,      -
   5        $
 controlB��5,   F   $ controlB_Click��6b   $ B��6X   6b     5-      -   5        $
 controlC��5-   F   $ controlC_Click��6b   $ C��6X   6b     5.      -   5        $
 	controlA1��5.   F   $ controlA1_Click��6d   $ A1��6Y   6d     5/      -   5        $
 	controlA2��5/   F   $ controlA2_Click��6d   $ A2��6Y   6d     50      -   5        $
 	controlA3��50   F   $ controlA3_Click��6d   $ A3��6Y   6d     51      -   5        $
 	controlA4��51   F   $ controlA4_Click��6d   $ A4��6Y   6d     52      -   5        $
 	controlA5��52   F   $ controlA5_Click��6d   $ A5��6Y   6d     53      -   5        $
 	controlA6��53   F   $ controlA6_Click��6d   $ A6��6Y   6d   54      �� 55      $��1��6%   $��Right=�65   $	��Top=�6Q   .+�56      $   CoRo.UserControlButtonSelector  -   5          Name$ controlHome��56   >   $   CoRo.UserControlButtonSelector   Click$ controlHome_Click��6`      Label$
 Home��6S   6`     57      -   5        $ controlSwitch��57   >   $ controlSwitch_Click��6i   $ SwitchRobot��6U   6i     58      -   5        $ controlGripper��58   >   $ controlGripper_Click��6f   $ Gripper��6V   6f     59      -   5        $ controlCooling��59   >   $ controlCooling_Click��6f   $ Cooling��6V   6f     5:      -   5        $ controlMode��5:   >   $ controlMode_Click��6`   $
 Mode��6S   6`     5;      -   5        $ controlConnect��5;   >   $ controlConnect_Click��6f   $ Connect��6V   6f   5<      5>      5?      5@      �� 5A      $ 
Horizontal=�6   $��Right=�62   $
��0,10q�6N   .+�5B      �� -   5        $	 Cancel��5B      $��30��6)   $��30��65   
������ 
�������$	��5,0q�6q   .��5C      �� -   5         ��Name$ cancel��5C      $ ��./Resources/cancelicon.png��6,   $	��5,5q�6P   5D      �� 5E      -   5        $	 OK��5E      $��30��6%   $��30��61   
������ 
�������.��5F      �� -   5        $ ok��5F      $��./Resources/okicon.png��6(   $	��5,5q�6H   5G      5H      5I      5J      5K      