   M S B A M L   `   `   `  ����  BasicSecurityProject*    $BasicSecurityProject.StenografieGrid   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      ���	#FFE5E5E5.+�5      �� -   5          ��Name$  
tabControl��5      $��Stretch=�6%   $��Stretch=�6C   .@�5   
   �� $��Encrypteren��6   .��5      � ���	#FFE5E5E5.+�5      � $��10,10,10,10q�6    � ColumnDefinitions 5	      �� 5
      $	��140 �6+   �� 5      $	��250 �6+   �� 5      $��2 �6+   �� 5      $
��auto �6+    � RowDefinitions 5      �� 5      $��23 �6(   �� 5      $��23 �6(   �� 5      $��23 �6(   �� 5      $��23 �6(   �� 5      $��23 �6(   .+�5      �� $��Encrypted File: ��6    $
��Left=�68   $��Center=�6S   $��0��6n   $��0��6~   $��16��6�   $��0,4,0,3q�6�   $��80��6�   �� 5      -   5         ��Name$ encryptedFileTextBox��5      $��Stretch=�6<    �� TextWrapping$
 Wrap=�6Z   $�� ��6n   $��Stretch=�6v   $��1��6�   $��0��6�    �� 
IsReadOnly . 6�   �� 5      -   5         ��Name$ encryptedFileButton��5      $	��...��6:   $
��Left=�6H   $��25��6c   $��3��6�   $��0��6�   $��23��6�   $��Bottom=�6�   �� 5      $��Image: ��6    $
��Left=�6/   $��Center=�6J   $��0��6e   $��3��6u   $��16��6�   $��0,3,0,4q�6�   $��53��6�   �� 5      -   5        $ imageTextBox��5      $
��Left=�64   $��23��6O   $
 Wrap=�6[   $�� ��6o   $	��Top=�6w   $	��250��6�   $��1��6�   $��3��6�    . 6�   �� 5      -   5        $ imageButton��5      $	��...��62   $
��Left=�6@   $��25��6[   $��3��6�   $��3��6�   $��23��6�   $��Bottom=�6�   �� 5      $��Encrypted Des Key: ��6    $
��Left=�6;   $��Center=�6V   $��0��6q   $��1��6�   $��16��6�   $��0,4,0,3q�6�   $	��104��6�   �� 5      -   5        $ encryptedKeyTextBox��5      $
��Left=�6;   $��23��6V   $
 Wrap=�6b   $�� ��6v   $	��Top=�6~   $	��250��6�   " �� VerticalScrollBarVisibility$
 Auto=�6�    . $��1��6�   $��1��6�   �� 5      -   5        $ encryptedKeyButton��5      $	��...��69   $
��Left=�6G   $	��Top=�6b   $��25��6z   $��23��6�   $��3��6�   $��1��6�   �� 5      $��Signed Hash: ��6    $
��Left=�65   $��Center=�6P   $��0��6k   $��2��6{   $��16��6�   $��0,4,0,3q�6�   $	��104��6�   �� 5       -   5        $ signedHashTextBox��5       $
��Left=�69   $��23��6T   $
 Wrap=�6`   $�� ��6t   $	��Top=�6|   $	��250��6�   $
 Auto=�6�    . $��1��6�   $��2��6�   �� 5!      -	   5        $ signedHashButton��5!      $	��...��67   $
��Left=�6E   $	��Top=�6`   $��25��6x   $��23��6�   $��3��6�   $��2��6�   �� 5"      -
   5        $ embedButton��5"      $��Embed��62   $
��Left=�6B   $��Stretch=�6]   $��75��6y   $��3��6�   $��4��6�   5#      5$      5%      �� 5&   
   $��Decrypteren��6   .��5'      � ���	#FFE5E5E5.+�5(      � $��10,10,10,10q�6    5)      �� 5*      $	��140 �6+   �� 5+      $	��250 �6+   �� 5,      $��2 �6+   �� 5-      $
��auto �6+    5/      �� 50      $��23 �6(   �� 51      $��23 �6(   �� 52      $��23 �6(   .+�54      �� $��Embedded Image: ��6    $
��Left=�68   $��Center=�6S   $��0��6n   $��0��6~   $��16��6�   $��0,4,0,3q�6�   $��80��6�   �� 55      -   5        $ embeddedImageTextBox��55      $��Stretch=�6<   $
 Wrap=�6Z   $�� ��6n   $��Stretch=�6v   $��1��6�   $��0��6�    . 6�   �� 56      -   5        $ embeddedImageButton��56      $	��...��6:   $
��Left=�6H   $��25��6c   $��3��6�   $��0��6�   $��23��6�   $��Bottom=�6�   �� 57      $��To folder: ��6    $
��Left=�63   $��Center=�6N   $��0��6i   $��1��6y   $��16��6�   $��0,3,0,4q�6�   $��53��6�   �� 58      -   5        $ decryptedFolderTextBox��58      $
��Left=�6>   $��23��6Y   $
 Wrap=�6e   $�� ��6y   $	��Top=�6�   $	��250��6�   $��1��6�   $��1��6�    . 6�   �� 59      -   5        $ decryptedFolderButton��59      $	��...��6<   $
��Left=�6J   $��25��6e   $��3��6�   $��1��6�   $��23��6�   $��Bottom=�6�   �� 5:      -   5        $ decryptButton��5:      $��Decrypt��64   $
��Left=�6F   $��Stretch=�6a   $��75��6}   $��3��6�   $��2��6�   5;      5<      5=      5>      5?      