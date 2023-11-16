--! Use IEEE library
LIBRARY ieee;

    --! Use IEEE standard logic
    USE ieee.std_logic_1164.ALL;

--! Half adder entity
ENTITY half_adder IS
    PORT (
        a_in  : IN    std_logic;
        b_in  : IN    std_logic;
        s_out : OUT   std_logic;
        c_out : OUT   std_logic
    );
END ENTITY half_adder;

--! Half adder RTL architecture
ARCHITECTURE rtl OF half_adder IS
BEGIN

    s_out <= a_in XOR b_in;
    c_out <= a_in AND b_in;

END ARCHITECTURE rtl;
