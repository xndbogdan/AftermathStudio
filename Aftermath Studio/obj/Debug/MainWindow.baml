   M S B A M L   `   `   `  ����f  bWPFSoundVisualizationLib, Version=0.3.4442.43191, Culture=neutral, PublicKeyToken=eb44ed7102b4fcc1eHclr-namespace:WPFSoundVisualizationLib;assembly=WPFSoundVisualizationLibWPFSoundVisualizationLib  5   	    Aftermath Studio!   Aftermath_Studio.MainWindow   eWPFSoundVisualizationLibHclr-namespace:WPFSoundVisualizationLib;assembly=WPFSoundVisualizationLib  W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  Studio_Main_Window��5   t    � Title$ Aftermath Studio��5   	   $	��232��6"   $��541.5��6/    � Icon$ 	Aicon.ico��6=   $	��510��6q   $	��300��6�    � 
ResizeMode$ CanMinimize=�6�   .��5      � -   5         �Name$ MainGrid��5      ���	#FF369E74 � ColumnDefinitions 5   
   �� 5      .+�5
   
   �� -   5         ��Name$ LoseIt��5
      $
��Left=�6"   $��85��6=   $��-56,-25,0,0q�6I    �� TextWrapping$
 Wrap=�6^   $��TextBox��6r   $	��Top=�6�   $��51��6�   ��6�   Z� ��6�   Z�  � 5   
   -   5          �Name$ Loading_label��5      $:��4Aftermath Studio is loading samples.
Please wait...��6'   $
��Left=�6n   $	��100��6�   $��61,159,0,-80q�6�   $	��Top=�6�   $	��377��6�   $y�	Collapsed=�6�   �� 5   
   -   5        $ Utilization_Box��5      $��	2,0,404,3q�6+   $
 Wrap=�6>   $��0��6R   $��23��6[   $��Bottom=�6g   $y�	Collapsed=�6�   � 5   
   -   5        $ MainBlur��5      $��0,0,0,3q�6!    5      �� 5      $��1* �6#   �� 5      $��1* �6#   �� 5      $��1* �6#   �� 5      $��1* �6#   .+�5      o� -   5        	 o�Name$
	 Menu��5      $��22��6!   $	��Top=�6-   ���	#FFF0F0F0$��4��6�   .@�5      m� -   5        
 m�Name$
 Studio_Menu��5      $
��File��60   ���	#FFF0F0F0.@�5      m� -	   5        $
 New_Project_Btn��5      $��New Project��68   6M   m� 5      -
   5        $
 Open_Project_Btn��5      $��Open Project��69   6O   �� 5      m� 5      -   5        $
 Save_Btn��5      $��Save Project��61   6G   m� 5      -   5        $
 Save_As_Btn��5      $��Save Project As...��64   6P   �� 5      m� 5      -   5        $
 Export��5      $��Export��6/   6?   �� 5      m� 5      -   5        $
 Quit_btn��5      $
��Quit��61   6?   5       m� 5!      $��Options��6   ���	#FFF0F0F0.@�5"      m� -   5        $
 Preferences_Btn��5"      $��Preferences��68   6M   5#      m� 5$      $
��Help��6   .@�5%      m� -   5        $
 About��5%      $��About��6.   6=   m� 5&      -   5        $
 Manual��5&      $��User Manual��6/   6D   5'      5(       � 5)      -   5        $�� ��5)      $��24��6   $��0,22,0,0q�6+   $	��Top=�6=   ���	#FF11171F$��4��6�   �� 5*      -   5        $ 	Timer_Box��5*      $��18��6)   $��0,24,3,0q�65   $
 Wrap=�6G   $	��Top=�6[   $��1 .  1��6s    �� TextAlignment$ Right=�6�   ���	#FFF9F9F9 �� 
IsReadOnly .  �� IsUndoEnabled .   �� IsReadOnlyCaretVisible . 
���   � � Cursor$ Arrow��6L   �� SelectionBrush
 �   � �� SelectionOpacity$ -1Q�6�  $��Right=�6�  $	��119��6�  $��1��6�  ��6�   Z� 6�  �� 5+      -   5        $ Activity��5+      $��18��6(   $��2,24,0,0q�64   $
 Wrap=�6F   $��Stopped��6Z   $	��Top=�6i   ���	#FFF9F9F9$
��Left=�6�   $��51��6�   {�.  ��6�   Z� 6�   �� 5,      -   5         ��Name$ Samples_Btn��5,      $��Samples��6*   $��	0,24,10,0q�6<   $��Right=�6O   $��75��6k   $��18��6v   $	��Top=�6�   ���	#FFA8A8A8 � ToolTip$" This selects a sample folder��6�   $��3��6�    � 5-      $�� ��6   $��24��6   $��2,51,2.084,0q�6+   $	��Top=�6A   
����������	#FF11171F6l    � 5.      $�� ��6   $��24��6   $��2.916,51,3,0q�6+   $	��Top=�6A   ���	#FF11171F
�������$��1��6�    � 5/      $�� ��6   $��24��6   $��2,51,2,0q�6+   $	��Top=�6=   ���	#FF11171F
�������$��2��6    � 50      $�� ��6   $��24��6   $��2,51,2,0q�6+   $	��Top=�6=   ���	#FF11171F
�������$��3��6   �� 51      -   5         ��Name$ B_label��51      $
��Bass��6%   $��52,51,49.084,0q�64   $	��Top=�6L   $��Center=�6d   $��24��6�   
�������$��34��6�   �� 52      -   5        $ D_label��52      $��Drums��6%   $��46.916,51,43,0q�65   $	��Top=�6M   $��Center=�6e   $��24��6�   
�������$��44��6�   $��1��6�   �� 53      -   5        $ L_label��53      $
��Lead��6%   $��Center=�64   $��
47,51,34,0q�6Q   $	��Top=�6f   $��24��6   
�������$��52��6�   $��2��6�   �� 54      -   5        $ V_label��54      $��Voice��6%   $��Center=�65   $��
45,51,37,0q�6R   $	��Top=�6f   $��24��6   
�������$��52��6�   $��3��6�   �� 55      -   5        $ Add_Pattern_Btn��55      $	��Add��6.   $��2,0,3.084,0q�6<   $��Bottom=�6Q   $��20��6�   $ Adds a pattern��6�   �� 56      -   5        $ Del_Pattern_Btn��56      $��Delete��6.   $��2.916,0,3,0q�6?   $��20��6r   $��Bottom=�6~   $ Deletes the last pattern��6�   $��1��6�    � 57      $�� ��6   $��16��6   $��	62,25,0,0q�6+   $	��Top=�6>   ���	#FFC4C642$
��Left=�6m   $��56��6�   �� 58      -   5        $ bpm_box��58      $
��Left=�6'   $��18��6B   $��	61,24,0,0q�6N   $
 Wrap=�6a   $	��120��6u   $	��Top=�6�   $��58��6�   $ Center=�6�   ���	#FFF9F9F9
���   �$ Arrow��6O   �	#FF3399FF��6�   Z� 6^  �� 59      -   5        $ 
sample_box��59      $��18��6*   $��	0,24,90,0q�66   $
 Wrap=�6I   $	��120��6]   $	��Top=�6h   $ Center=�6�   ���	#FFF9F9F9
���   �$ Arrow��6�    .  . $��Right=�6  $��58��63  $��2��6>  $��2��6R  ��6�   Z� 6R  � 5:      -   5        $ Text_Grid_Pat��5:      $	��3,0q�6*   $��23��67   $��Bottom=�6C   $��2��6^    5;      �� 5<      �� 5=      .+�5?      �� -   5        $ Current_Pat��5?      $��1��6/   $��20��68   $��Bottom=�6D   $ Center=�6_   $��0,0,2,0q�6�   �� 5@      -    5        $ Max_Pat��5@      $��/0��6+   $��20��65   $��Bottom=�6A   $ Center=�6\   {�.  
����������	#FF707070$��1��6�   $��3,0,0,0q�6�   $ Number of patterns��6�   5A      � 5B      -!   5        $ Button_Grid��5B      $	��2,0q�6(   $	��100��65   $��Bottom=�6B   $��3��6]    5C      �� 5D      �� 5E      .+�5G      �� -"   5        $ Inc_Pat��5G      $
��Next��6*   $��20��69   $��Bottom=�6E   $��1��6v   $��4,0,0,0q�6�   $ Go to the next pattern��6�   �� 5H      -#   5        $ Dec_Pat��5H      $��Previous��6*   $��20��6=   $��Bottom=�6I   $��0,0,3,0q�6z   $  Go to the previous pattern��6�   5I       � 5J      -$   5        $ Record_Frame��5J      $�� ��6*   $��18��65   $��	0,24,58,0q�6A   $	��Top=�6T   $��Right=�6l   $��18��6�   $��2��6�   �� 5K      -%   5         ��Name$ 
Record_Btn��5K      $��2��6(   $��Right=�68   $��18��6T   $��	0,24,58,0q�6`   $	��Top=�6s   $��18��6�   $��Record_BTN.png��6�   6�    � 5L      -&   5        $ 
Play_Frame��5L      $�� ��6(   $��18��63   $��
0,24,104,0q�6?   $	��Top=�6S   $��Right=�6k   $��18��6�   $��2��6�    � 5M      -'   5        $ 
Stop_Frame��5M      $�� ��6(   $��18��63   $��	0,24,81,0q�6?   $	��Top=�6R   $��Right=�6j   $��18��6�   $��2��6�   �� 5N      -(   5        $ Play_Btn��5N      $��Right=�6&   $��18��6B   $��
0,24,104,0q�6N   $	��Top=�6b   $��18��6z   $��Play_BTN.png��6�   $��2��6  6   �� 5O      -)   5        $ Stop_Btn��5O      $��Right=�6&   $��18��6B   $��	0,24,81,0q�6N   $	��Top=�6a   $��18��6y   $��Stop_Btn.png��6�   $��2��6  �� 5P      -*   5        $ 	B_samples��5P      $��0��6'   $��0,51,3.084,0q�63   $	��Top=�6I   $��Right=�6a   $��24��6}   
�������$��34��6�   �� 5Q      -+   5        $ 	D_samples��5Q      $��0��6'   $��0,51,3,0q�63   $	��Top=�6E   $��Right=�6]   $��24��6y   
�������$��34��6�   $��1��6�   �� 5R      -,   5        $ 	L_samples��5R      $��0��6'   $��0,51,3,0q�63   $	��Top=�6E   $��Right=�6]   $��24��6y   
�������$��34��6�   $��2��6�   �� 5S      --   5        $ 	V_samples��5S      $��0��6'   $��0,51,3,0q�63   $	��Top=�6E   $��Right=�6]   $��24��6y   
�������$��34��6�   $��3��6�   �� 5T      -.   5         ��Name$ 
Visualizer��5T      
�������$��
2,108,2,25q�69   $��4��6M   �� 5U      -/   5        $
 VSQ1��5U      ���	#FF77FF71$��2,0,0,25q�67   $��20��6I   $��Bottom=�6U   �� 5V      -0   5        $
 VSQ2��5V      ���	#FF77FF71$��0,0,0,25q�67   $��20��6I   $��Bottom=�6U   $��1��6p   �� 5W      -1   5        $
 VSQ3��5W      ���	#FF77FF71$��0,0,0,25q�67   $��20��6I   $��Bottom=�6U   $��2��6p   �� 5X      -2   5        $
 VSQ4��5X      ���	#FF77FF71$��0,0,2,25q�67   $��20��6I   $��Bottom=�6U   $��3��6p   �� 5Y      -3   5         ��Name$ Bass_Box��5Y      $��2,80,3.084,0q�6)   $	��Top=�6?   $��23��6W   6c   �� 5Z      -4   5        $ 	Drums_Box��5Z      $��2.916,80,3,0q�6*   $	��Top=�6@   $��23��6X   $��1��6�   �� 5[      -5   5        $ Lead_Box��5[      $��2,80,2,0q�6)   $	��Top=�6;   $��23��6S   $��2��6�   �� 5\      -6   5        $ 	Voice_Box��5\      $��2,80,2,0q�6*   $	��Top=�6<   $��23��6T   $��3��6�   5_      � 5`   
   -7   5        $ Render_Grid��5`      $��10q�6$   $y�	Collapsed=�60    � RowDefinitions 5a      �� 5b      �� 5c      $��31 �6    �� 5d      .+�5f      �� -8   5        $ 
Export_Btn��5f      $��Export��6)   $
��Left=�6:   $��0,0,0,10q�6U   $��Bottom=�6g   $	��112��6�   $��2��6�   $��20��6�   �� 5g      -9   5        $ 
Cancel_Btn��5g      $��Cancel��6)   $��Right=�6:   $��0,0,0,10q�6V   $��Bottom=�6h   $	��122��6�   $��2��6�   $��20��6�   � 5h      -:   5         �Name$ Exp_Bar��5h      $��0,6,0,0q�6+   $��1��6<   $��25��6I   $	��Top=�6U   $��2��6m   �� 5i      -;   5         ��Name$ 	Exp_label��5i      $
��Wrap=�6+   $��	Exporting��6?   $	��Top=�6P   $��21��6h    �� TextAlignment$ Center=�6t   $��0,8,26,0q�6�   $��1��6�   5j      5k      5m      