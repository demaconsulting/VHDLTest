--! Use IEEE library
LIBRARY ieee;

    --! Use IEEE standard logic
    USE ieee.std_logic_1164.ALL;

--! Full adder entity
ENTITY full_adder IS
    PORT (
        a_in  : IN    std_logic;
        b_in  : IN    std_logic;
        c_in  : IN    std_logic;
        s_out : OUT   std_logic;
        c_out : OUT   std_logic
    );
END ENTITY full_adder;

--! Full adder RTL architecture
ARCHITECTURE rtl OF full_adder IS
BEGIN

    s_out <= a_in XOR b_in XOR c_in;
    c_out <= (a_in AND b_in) OR (c_in AND a_in) OR (c_in AND b_in);

END ARCHITECTURE rtl;
