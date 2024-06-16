CREATE OR REPLACE FUNCTION update_date_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.update_date := CURRENT_TIMESTAMP; 
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_date_trigger
BEFORE UPDATE ON troubleshooting_cost_node
FOR EACH ROW
EXECUTE PROCEDURE update_date_column();